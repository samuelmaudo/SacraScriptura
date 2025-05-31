using Microsoft.EntityFrameworkCore;
using SacraScriptura.Admin.Domain.Books;
using SacraScriptura.Admin.Domain.Divisions;

namespace SacraScriptura.Admin.Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for managing Division entities using the Nested Sets pattern.
/// </summary>
public class DivisionRepository(ApplicationDbContext context) : IDivisionRepository
{
    /// <summary>
    /// Gets all divisions.
    /// </summary>
    public async Task<IEnumerable<Division>> GetAllAsync()
    {
        return await context
            .Divisions.OrderBy(d => d.BookId)
            .ThenBy(d => d.LeftValue)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a division by its ID.
    /// </summary>
    public async Task<Division?> GetByIdAsync(DivisionId id)
    {
        return await context.Divisions.FindAsync(id);
    }

    /// <summary>
    /// Gets all divisions for a specific book.
    /// </summary>
    public async Task<IEnumerable<Division>> GetByBookIdAsync(BookId bookId)
    {
        return await context
            .Divisions.Where(d => d.BookId == bookId)
            .OrderBy(d => d.LeftValue)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all divisions for a specific book, ordered hierarchically.
    /// </summary>
    public async Task<IEnumerable<Division>> GetHierarchyByBookIdAsync(BookId bookId)
    {
        // Obtener todas las divisiones del libro ordenadas por LeftValue
        // Esto garantiza que los padres aparezcan antes que sus hijos
        var divisions = await context
            .Divisions.Where(d => d.BookId == bookId)
            .OrderBy(d => d.LeftValue)
            .ToListAsync();

        // Construir la jerarquía en memoria
        var divisionMap = divisions.ToDictionary(d => d.Id);
        var rootDivisions = new List<Division>();

        foreach (var division in divisions)
        {
            // Si no tiene ParentId, es una división raíz
            if (division.ParentId == null)
            {
                rootDivisions.Add(division);
                continue;
            }

            // Agregar esta división como hijo de su padre
            if (divisionMap.TryGetValue(division.ParentId, out var parent))
            {
                parent.Children.Add(division);
                division.Parent = parent;
            }
        }

        return rootDivisions;
    }

    /// <summary>
    /// Gets all children of a specific division.
    /// </summary>
    public async Task<IEnumerable<Division>> GetChildrenAsync(DivisionId parentId)
    {
        var parent = await context.Divisions.FindAsync(parentId);
        if (parent == null)
        {
            return Enumerable.Empty<Division>();
        }

        // En el patrón Nested Sets, los hijos directos son aquellos cuyo padre es el nodo actual
        return await context
            .Divisions.Where(d => d.ParentId == parentId)
            .OrderBy(d => d.Order)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all ancestors of a specific division.
    /// </summary>
    public async Task<IEnumerable<Division>> GetAncestorsAsync(DivisionId divisionId)
    {
        var division = await context.Divisions.FindAsync(divisionId);
        if (division == null)
        {
            return Enumerable.Empty<Division>();
        }

        // En el patrón Nested Sets, los ancestros son aquellos nodos que contienen al nodo actual
        // Es decir, left < division.left y right > division.right
        return await context
            .Divisions.Where(d =>
                d.BookId == division.BookId
                && d.LeftValue < division.LeftValue
                && d.RightValue > division.RightValue
            )
            .OrderBy(d => d.LeftValue)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all descendants of a specific division.
    /// </summary>
    public async Task<IEnumerable<Division>> GetDescendantsAsync(DivisionId divisionId)
    {
        var division = await context.Divisions.FindAsync(divisionId);
        if (division == null)
        {
            return Enumerable.Empty<Division>();
        }

        // En el patrón Nested Sets, los descendientes son aquellos nodos contenidos dentro del nodo actual
        // Es decir, left > division.left y right < division.right
        return await context
            .Divisions.Where(d =>
                d.BookId == division.BookId
                && d.LeftValue > division.LeftValue
                && d.RightValue < division.RightValue
            )
            .OrderBy(d => d.LeftValue)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new division.
    /// </summary>
    public async Task AddAsync(Division division)
    {
        // Si no hay divisiones para este libro, esta será la primera
        var anyDivisions = await context.Divisions.AnyAsync(d => d.BookId == division.BookId);

        if (!anyDivisions)
        {
            // Primera división del libro (raíz)
            division.LeftValue = 1;
            division.RightValue = 2;
            division.Depth = 0;
            division.Order = 0;
        }
        else
        {
            // Si no se especifica un padre, se añade como hermano de las divisiones raíz existentes
            var maxRightValue = await context
                .Divisions.Where(d => d.BookId == division.BookId && d.Depth == 0)
                .MaxAsync(d => d.RightValue);

            var maxOrder = await context
                .Divisions.Where(d => d.BookId == division.BookId && d.Depth == 0)
                .MaxAsync(d => d.Order);

            // Actualizar todos los nodos con RightValue >= maxRightValue
            await UpdateRightValuesAsync(division.BookId, maxRightValue, 2);

            // Establecer valores para la nueva división
            division.LeftValue = maxRightValue;
            division.RightValue = maxRightValue + 1;
            division.Depth = 0;
            division.Order = maxOrder + 1;
        }

        await context.Divisions.AddAsync(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new division as a child of a parent division.
    /// </summary>
    public async Task AddAsChildAsync(Division division, DivisionId parentId)
    {
        // Por defecto, añadir como último hijo
        await AddAsLastChildAsync(division, parentId);
    }

    /// <summary>
    /// Adds a new division as the first child of a parent division.
    /// </summary>
    public async Task AddAsFirstChildAsync(Division division, DivisionId parentId)
    {
        var parent =
            await context.Divisions.FindAsync(parentId)
            ?? throw new ArgumentException(
                $"Parent division with ID {parentId} not found",
                nameof(parentId)
            );

        // Verificar que la división pertenece al mismo libro que el padre
        division.BookId = parent.BookId;
        division.ParentId = parentId;
        division.Depth = parent.Depth + 1;

        // En el patrón Nested Sets, para insertar como primer hijo:
        // 1. El nuevo nodo tendrá LeftValue = parent.LeftValue + 1
        // 2. Incrementar LeftValue y RightValue de todos los nodos con LeftValue > parent.LeftValue
        // 3. Incrementar RightValue de todos los nodos con RightValue >= parent.LeftValue + 1

        // Actualizar los valores de los nodos existentes
        await UpdateRightValuesAsync(parent.BookId, parent.LeftValue + 1, 2);

        // Establecer valores para la nueva división
        division.LeftValue = parent.LeftValue + 1;
        division.RightValue = parent.LeftValue + 2;
        division.Order = 0;

        // Incrementar Order de los hermanos existentes
        await context
            .Divisions.Where(d => d.ParentId == parentId && d.Id != division.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order + 1));

        await context.Divisions.AddAsync(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new division as the last child of a parent division.
    /// </summary>
    public async Task AddAsLastChildAsync(Division division, DivisionId parentId)
    {
        var parent =
            await context.Divisions.FindAsync(parentId)
            ?? throw new ArgumentException(
                $"Parent division with ID {parentId} not found",
                nameof(parentId)
            );

        // Verificar que la división pertenece al mismo libro que el padre
        division.BookId = parent.BookId;
        division.ParentId = parentId;
        division.Depth = parent.Depth + 1;

        // Obtener el máximo orden entre los hijos existentes
        var maxOrder = await context
            .Divisions.Where(d => d.ParentId == parentId)
            .Select(d => d.Order)
            .DefaultIfEmpty(-1)
            .MaxAsync();

        division.Order = maxOrder + 1;

        // En el patrón Nested Sets, para insertar como último hijo:
        // 1. El nuevo nodo tendrá LeftValue = parent.RightValue - 1
        // 2. Incrementar RightValue de todos los nodos con RightValue >= parent.RightValue
        // 3. Incrementar LeftValue de todos los nodos con LeftValue > parent.RightValue - 1

        // Actualizar los valores de los nodos existentes
        await UpdateRightValuesAsync(parent.BookId, parent.RightValue, 2);

        // Establecer valores para la nueva división
        division.LeftValue = parent.RightValue - 1;
        division.RightValue = parent.RightValue;

        await context.Divisions.AddAsync(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new division as a sibling before the specified division.
    /// </summary>
    public async Task AddBeforeAsync(Division division, DivisionId siblingId)
    {
        var sibling =
            await context.Divisions.FindAsync(siblingId)
            ?? throw new ArgumentException(
                $"Sibling division with ID {siblingId} not found",
                nameof(siblingId)
            );

        // Verificar que la división pertenece al mismo libro que el hermano
        division.BookId = sibling.BookId;
        division.ParentId = sibling.ParentId;
        division.Depth = sibling.Depth;

        // En el patrón Nested Sets, para insertar antes de un hermano:
        // 1. El nuevo nodo tendrá LeftValue = sibling.LeftValue
        // 2. Incrementar LeftValue y RightValue de todos los nodos con LeftValue >= sibling.LeftValue
        // 3. Incrementar RightValue de todos los nodos con RightValue > sibling.LeftValue

        // Actualizar los valores de los nodos existentes
        await UpdateRightValuesAsync(sibling.BookId, sibling.LeftValue, 2);

        // Establecer valores para la nueva división
        division.LeftValue = sibling.LeftValue;
        division.RightValue = sibling.LeftValue + 1;
        division.Order = sibling.Order;

        // Incrementar Order de los hermanos a partir del actual
        await context
            .Divisions.Where(d =>
                d.ParentId == sibling.ParentId && d.Order >= sibling.Order && d.Id != division.Id
            )
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order + 1));

        await context.Divisions.AddAsync(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new division as a sibling after the specified division.
    /// </summary>
    public async Task AddAfterAsync(Division division, DivisionId siblingId)
    {
        var sibling =
            await context.Divisions.FindAsync(siblingId)
            ?? throw new ArgumentException(
                $"Sibling division with ID {siblingId} not found",
                nameof(siblingId)
            );

        // Verificar que la división pertenece al mismo libro que el hermano
        division.BookId = sibling.BookId;
        division.ParentId = sibling.ParentId;
        division.Depth = sibling.Depth;

        // En el patrón Nested Sets, para insertar después de un hermano:
        // 1. El nuevo nodo tendrá LeftValue = sibling.RightValue + 1
        // 2. Incrementar LeftValue y RightValue de todos los nodos con LeftValue > sibling.RightValue
        // 3. Incrementar RightValue de todos los nodos con RightValue > sibling.RightValue

        // Actualizar los valores de los nodos existentes
        await UpdateRightValuesAsync(sibling.BookId, sibling.RightValue + 1, 2);

        // Establecer valores para la nueva división
        division.LeftValue = sibling.RightValue + 1;
        division.RightValue = sibling.RightValue + 2;
        division.Order = sibling.Order + 1;

        // Incrementar Order de los hermanos posteriores
        await context
            .Divisions.Where(d =>
                d.ParentId == sibling.ParentId && d.Order > sibling.Order && d.Id != division.Id
            )
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order + 1));

        await context.Divisions.AddAsync(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates a division.
    /// </summary>
    public async Task UpdateAsync(Division division)
    {
        context.Divisions.Update(division);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Moves a division to be a child of a new parent.
    /// </summary>
    public async Task MoveToChildOfAsync(DivisionId divisionId, DivisionId newParentId)
    {
        var division =
            await context.Divisions.FindAsync(divisionId)
            ?? throw new ArgumentException(
                $"Division with ID {divisionId} not found",
                nameof(divisionId)
            );

        var newParent =
            await context.Divisions.FindAsync(newParentId)
            ?? throw new ArgumentException(
                $"New parent division with ID {newParentId} not found",
                nameof(newParentId)
            );

        // Verificar que ambas divisiones pertenecen al mismo libro
        if (division.BookId != newParent.BookId)
        {
            throw new ArgumentException("Division and new parent must belong to the same book");
        }

        // Verificar que el nuevo padre no es descendiente de la división (evitar ciclos)
        if (newParent.LeftValue > division.LeftValue && newParent.RightValue < division.RightValue)
        {
            throw new ArgumentException("Cannot move a division to one of its descendants");
        }

        // Calcular el tamaño del subárbol que se va a mover
        int nodeSize = division.RightValue - division.LeftValue + 1;

        // Crear espacio en el nuevo destino
        await UpdateRightValuesAsync(division.BookId, newParent.RightValue, nodeSize);

        // Calcular el desplazamiento para los nodos del subárbol
        int leftOffset,
            rightOffset;

        // Si movemos hacia adelante en el árbol
        if (newParent.RightValue > division.RightValue)
        {
            leftOffset = newParent.RightValue - division.LeftValue - nodeSize;
            rightOffset = leftOffset;
        }
        // Si movemos hacia atrás en el árbol
        else
        {
            leftOffset = newParent.RightValue - division.LeftValue;
            rightOffset = leftOffset;
        }

        // Actualizar la profundidad del subárbol
        int depthDiff = newParent.Depth + 1 - division.Depth;

        // Obtener todos los nodos del subárbol que se va a mover
        var subtreeNodes = await context
            .Divisions.Where(d =>
                d.BookId == division.BookId
                && d.LeftValue >= division.LeftValue
                && d.RightValue <= division.RightValue
            )
            .ToListAsync();

        // Actualizar temporalmente los valores a negativos para evitar conflictos
        foreach (var node in subtreeNodes)
        {
            node.LeftValue = -node.LeftValue;
            node.RightValue = -node.RightValue;
            node.Depth += depthDiff;
        }

        await context.SaveChangesAsync();

        // Actualizar a los valores finales
        foreach (var node in subtreeNodes)
        {
            node.LeftValue = -node.LeftValue + leftOffset;
            node.RightValue = -node.RightValue + rightOffset;
        }

        // Actualizar el ParentId de la división que se mueve
        division.ParentId = newParentId;

        // Actualizar el orden entre hermanos
        var maxOrder = await context
            .Divisions.Where(d => d.ParentId == newParentId && d.Id != divisionId)
            .Select(d => d.Order)
            .DefaultIfEmpty(-1)
            .MaxAsync();

        division.Order = maxOrder + 1;

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Moves a division to be before a sibling.
    /// </summary>
    public async Task MoveBeforeAsync(DivisionId divisionId, DivisionId siblingId)
    {
        var division =
            await context.Divisions.FindAsync(divisionId)
            ?? throw new ArgumentException(
                $"Division with ID {divisionId} not found",
                nameof(divisionId)
            );

        var sibling =
            await context.Divisions.FindAsync(siblingId)
            ?? throw new ArgumentException(
                $"Sibling division with ID {siblingId} not found",
                nameof(siblingId)
            );

        // Verificar que ambas divisiones pertenecen al mismo libro
        if (division.BookId != sibling.BookId)
        {
            throw new ArgumentException("Division and sibling must belong to the same book");
        }

        // Si la división ya está antes del hermano, no hacer nada
        if (division.ParentId == sibling.ParentId && division.Order < sibling.Order)
        {
            return;
        }

        // Primero mover la división para que sea hijo del mismo padre que el hermano
        if (division.ParentId != sibling.ParentId)
        {
            var parentId =
                sibling.ParentId
                ?? throw new InvalidOperationException("Sibling must have a parent");
            await MoveToChildOfAsync(divisionId, parentId);

            // Recargar la división después de moverla
            division = await context.Divisions.FindAsync(divisionId);
        }

        // Actualizar el orden de las divisiones
        if (division.Order > sibling.Order)
        {
            // Incrementar el orden de las divisiones entre el hermano y la división
            await context
                .Divisions.Where(d =>
                    d.ParentId == sibling.ParentId
                    && d.Order >= sibling.Order
                    && d.Order < division.Order
                    && d.Id != divisionId
                )
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order + 1));

            // Actualizar el orden de la división
            division.Order = sibling.Order;
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Moves a division to be after a sibling.
    /// </summary>
    public async Task MoveAfterAsync(DivisionId divisionId, DivisionId siblingId)
    {
        var division =
            await context.Divisions.FindAsync(divisionId)
            ?? throw new ArgumentException(
                $"Division with ID {divisionId} not found",
                nameof(divisionId)
            );

        var sibling =
            await context.Divisions.FindAsync(siblingId)
            ?? throw new ArgumentException(
                $"Sibling division with ID {siblingId} not found",
                nameof(siblingId)
            );

        // Verificar que ambas divisiones pertenecen al mismo libro
        if (division.BookId != sibling.BookId)
        {
            throw new ArgumentException("Division and sibling must belong to the same book");
        }

        // Si la división ya está después del hermano, no hacer nada
        if (division.ParentId == sibling.ParentId && division.Order > sibling.Order)
        {
            return;
        }

        // Primero mover la división para que sea hijo del mismo padre que el hermano
        if (division.ParentId != sibling.ParentId)
        {
            var parentId =
                sibling.ParentId
                ?? throw new InvalidOperationException("Sibling must have a parent");
            await MoveToChildOfAsync(divisionId, parentId);

            // Recargar la división después de moverla
            division = await context.Divisions.FindAsync(divisionId);
        }

        // Actualizar el orden de las divisiones
        if (division.Order < sibling.Order)
        {
            // Decrementar el orden de las divisiones entre la división y el hermano
            await context
                .Divisions.Where(d =>
                    d.ParentId == sibling.ParentId
                    && d.Order > division.Order
                    && d.Order <= sibling.Order
                    && d.Id != divisionId
                )
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order - 1));

            // Actualizar el orden de la división
            division.Order = sibling.Order;
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Deletes a division and all its descendants.
    /// </summary>
    public async Task DeleteAsync(DivisionId id)
    {
        var division =
            await context.Divisions.FindAsync(id)
            ?? throw new ArgumentException($"Division with ID {id} not found", nameof(id));

        // Calcular el tamaño del subárbol que se va a eliminar
        int nodeSize = division.RightValue - division.LeftValue + 1;

        // Eliminar la división y todos sus descendientes
        await context
            .Divisions.Where(d =>
                d.BookId == division.BookId
                && d.LeftValue >= division.LeftValue
                && d.RightValue <= division.RightValue
            )
            .ExecuteDeleteAsync();

        // Actualizar los valores de los nodos restantes
        await context
            .Divisions.Where(d => d.BookId == division.BookId && d.RightValue > division.RightValue)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(d => d.RightValue, d => d.RightValue - nodeSize)
            );

        await context
            .Divisions.Where(d => d.BookId == division.BookId && d.LeftValue > division.RightValue)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.LeftValue, d => d.LeftValue - nodeSize));

        // Reordenar los hermanos si es necesario
        if (division.ParentId != null)
        {
            await context
                .Divisions.Where(d => d.ParentId == division.ParentId && d.Order > division.Order)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.Order, d => d.Order - 1));
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Rebuilds the nested sets values for a book's divisions.
    /// </summary>
    public async Task RebuildTreeAsync(BookId bookId)
    {
        // Obtener todas las divisiones del libro
        var divisions = await context
            .Divisions.Where(d => d.BookId == bookId)
            .OrderBy(d => d.Depth)
            .ThenBy(d => d.Order)
            .ToListAsync();

        if (!divisions.Any())
        {
            return;
        }

        // Construir un diccionario para acceder rápidamente a las divisiones por su ID
        var divisionMap = divisions.ToDictionary(d => d.Id);

        // Identificar las divisiones raíz (sin padre)
        var rootDivisions = divisions.Where(d => d.ParentId == null).ToList();

        // Construir la estructura jerárquica en memoria
        foreach (var division in divisions.Where(d => d.ParentId != null))
        {
            if (divisionMap.TryGetValue(division.ParentId, out var parent))
            {
                parent.Children.Add(division);
                division.Parent = parent;
            }
        }

        // Reconstruir los valores de Nested Sets
        int counter = 1;
        foreach (var root in rootDivisions.OrderBy(d => d.Order))
        {
            counter = RebuildNodeValues(root, counter, 0);
        }

        // Guardar los cambios
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Reconstruye recursivamente los valores de Nested Sets para un nodo y sus descendientes
    /// </summary>
    private int RebuildNodeValues(Division node, int leftValue, int depth)
    {
        // Asignar el valor izquierdo y la profundidad
        node.LeftValue = leftValue++;
        node.Depth = depth;

        // Procesar los hijos ordenados por Order
        foreach (var child in node.Children.OrderBy(c => c.Order))
        {
            leftValue = RebuildNodeValues(child, leftValue, depth + 1);
        }

        // Asignar el valor derecho
        node.RightValue = leftValue++;

        return leftValue;
    }

    /// <summary>
    /// Actualiza los valores Right de los nodos afectados al insertar un nuevo nodo
    /// </summary>
    private async Task UpdateRightValuesAsync(BookId bookId, int fromValue, int increment)
    {
        // Actualizar RightValue de todos los nodos con RightValue >= fromValue
        await context
            .Divisions.Where(d => d.BookId == bookId && d.RightValue >= fromValue)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(d => d.RightValue, d => d.RightValue + increment)
            );

        // Actualizar LeftValue de todos los nodos con LeftValue > fromValue
        await context
            .Divisions.Where(d => d.BookId == bookId && d.LeftValue > fromValue)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.LeftValue, d => d.LeftValue + increment));
    }
}
