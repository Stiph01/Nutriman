using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    [Header("Dimensiones del mapa")]
    public int width = 30;
    public int height = 30;

    [Header("Tilemap de paredes")]
    public Tilemap wallTilemap;
    public TileBase wallTile;

    [Header("Generación")]
    public bool generateOnStart = true;

    [Header("Zona central libre")]
    public int centralWidth = 8;
    public int centralHeight = 6;

    private int[,] grid;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateMaze();
        }
    }

    [ContextMenu("Generar Laberinto")]
    public void GenerateMaze()
    {
        ClearWalls();
        CreateGrid();
        CreateOuterWalls();
        CreateInnerWalls();
        ClearCentralZone();
        RenderWalls();
    }

    // ==========================================
    // BORRAR LAS PAREDES ANTERIORES
    // ==========================================

    private void ClearWalls()
    {
        if (wallTilemap != null)
        {
            wallTilemap.ClearAllTiles();
        }
    }

    // ==========================================
    // CREAR MATRIZ
    // 0 = ESPACIO LIBRE
    // 1 = PARED
    // ==========================================

    private void CreateGrid()
    {
        grid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = 0;
            }
        }
    }

    // ==========================================
    // PAREDES EXTERIORES
    // ==========================================

    private void CreateOuterWalls()
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = 1;
            grid[x, height - 1] = 1;
        }

        for (int y = 0; y < height; y++)
        {
            grid[0, y] = 1;
            grid[width - 1, y] = 1;
        }
    }

    // ==========================================
    // PAREDES INTERNAS
    //
    // Cada bloque es de 2x2.
    // Entre bloques dejamos 2 celdas libres.
    // ==========================================

    private void CreateInnerWalls()
    {
        // Posiciones de los bloques.
        // El patrón 2x2 + 2 espacios crea
        // corredores de mínimo 2 celdas.

        int[] positionsX =
        {
            2, 6, 10, 14, 18, 22, 26
        };

        int[] positionsY =
        {
            2, 6, 10, 14, 18, 22, 26
        };

        foreach (int x in positionsX)
        {
            foreach (int y in positionsY)
            {
                CreateWallBlock(x, y);
            }
        }

        // ======================================
        // ABRIR ALGUNOS BLOQUES
        // para que el diseño sea menos repetitivo
        // ======================================

        RemoveBlock(6, 6);
        RemoveBlock(18, 6);

        RemoveBlock(10, 14);
        RemoveBlock(22, 14);

        RemoveBlock(6, 22);
        RemoveBlock(18, 22);
    }

    // ==========================================
    // CREAR BLOQUE DE 2x2
    // ==========================================

    private void CreateWallBlock(int startX, int startY)
    {
        for (int x = startX; x < startX + 2; x++)
        {
            for (int y = startY; y < startY + 2; y++)
            {
                if (IsInsideMap(x, y))
                {
                    grid[x, y] = 1;
                }
            }
        }
    }

    // ==========================================
    // ELIMINAR BLOQUE DE 2x2
    // ==========================================

    private void RemoveBlock(int startX, int startY)
    {
        for (int x = startX; x < startX + 2; x++)
        {
            for (int y = startY; y < startY + 2; y++)
            {
                if (IsInsideMap(x, y))
                {
                    grid[x, y] = 0;
                }
            }
        }
    }

    // ==========================================
    // ZONA CENTRAL LIBRE
    // ==========================================

    private void ClearCentralZone()
    {
        int centerX = width / 2;
        int centerY = height / 2;

        int startX = centerX - centralWidth / 2;
        int endX = centerX + centralWidth / 2;

        int startY = centerY - centralHeight / 2;
        int endY = centerY + centralHeight / 2;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (IsInsideMap(x, y))
                {
                    grid[x, y] = 0;
                }
            }
        }
    }

    // ==========================================
    // COMPROBAR QUE ESTÉ DENTRO DEL MAPA
    // ==========================================

    private bool IsInsideMap(int x, int y)
    {
        return x >= 0 &&
               x < width &&
               y >= 0 &&
               y < height;
    }

    // ==========================================
    // DIBUJAR EN TILEMAP
    // ==========================================

    private void RenderWalls()
    {
        if (wallTilemap == null || wallTile == null)
        {
            Debug.LogWarning(
                "MazeGenerator: falta asignar Wall Tilemap o Wall Tile."
            );

            return;
        }

        int offsetX = width / 2;
        int offsetY = height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                {
                    Vector3Int tilePosition = new Vector3Int(
                        x - offsetX,
                        y - offsetY,
                        0
                    );

                    wallTilemap.SetTile(
                        tilePosition,
                        wallTile
                    );
                }
            }
        }
    }
}