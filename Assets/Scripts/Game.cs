using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.InputSystem;
public class Game : MonoBehaviour
{

    private Control control;
    private InputAction left;
    private InputAction right;
    private InputAction spin;
    private InputAction instantFall;

    // Move all unlocked blocks down by 1 every time this number of seconds passes.
    public static float GameSpeed { get; set; } = 48f;

    private static float[] gameSpeedByLevel = { 48, 43, 38, 33, 28, 23, 18, 13, 8, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 };

    //The board is formed by an array of block objects.
    public static Block[,] Board { get; set; } = new Block[10, 21];

    //The object to instantiate when we call instantiate.
    [SerializeField] private Block block;

    //Stores the current Piece object being used by the game.
    [SerializeField] private Piece piece;

    //A pool of block objects to save space on things.
    private static readonly Block[] blockPool = new Block[256];

    //The current index in the block pool to check. Note that it is a byte.
    private static readonly Queue<byte> poolIndex = new Queue<byte>();

    //The game score.
    public static long Score { get; set; } = 0;

    //The game level.
    public static int Level { get; set; } = 0;

    //The number of lines cleared this game.
    public static int TotalLinesCleared { get; set; } = 0;

    //The rotation of the board transformation.
    public static Quaternion BoardRotation { get; set; }

    //The game over text object.
    [SerializeField] private TMP_Text gameOverText;

    //Whether or not the game is over.
    public static bool GameOver { get; set; } = false;

    //These variables are for the animation that plays on the board when the game is over.
    //The current x-coordinate that is being filled with a block.
    private int gameOverDisplayX = 0;
    //The current y-coordinate that is being filled with a block.
    private int gameOverDisplayY = 1;
    //The time elapsed since the last block was added to or removed from the display.
    private float gameOverDisplayTime = 0f;
    //The amount of time in between each block being added to or removed from the display.
    private readonly float gameOverDisplayRate = 0.01f;
    //Whether the display is adding or removing blocks from the display, false = adding, true = removing.
    private bool gameOverDisplayPhase = false;

    
    //An input action object for the start button.
    private InputAction start;


    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        for(int i = 0; i < 256; i++)
        {
            poolIndex.Enqueue((byte)i);
        }

        InitiateBlockPool();

        control = new Control();

        control = new Control();
        control.Main.Left.performed += goLeft => piece.TranslatePieceHorizontal(true, true);
        control.Main.Right.performed += goRight => piece.TranslatePieceHorizontal(false, true);
        control.Main.Spin.performed += spinKey => piece.SpinPiece();
        control.Main.InstantFall.performed += instantFallKey => piece.IsFastFalling = 2f;
        control.Main.InstantFall.canceled += instantFallKeyOff => piece.IsFastFalling = 1f;
        control.Main.Start.performed += startButton => StartGame();
    }

    /// <summary>
    /// Populates BlockPool with inactive block objects.
    /// </summary>
    private void InitiateBlockPool()
    {
        //Note that the array is indexed with a byte, so it goes back to the beginning automatically when reaching the end.
        for (int i = 0; i < 256; i++)
        {
            blockPool[i] = Instantiate(block);
            blockPool[i].gameObject.SetActive(false);
            blockPool[i].GetComponent<VisualEffect>().Stop();
            blockPool[i].Index = (byte)i;
        }
    }

    //OnEnable is called when the object is activated or enabled.
    private void OnEnable()
    {
        start = control.Main.Start;
        start.Enable();

        left = control.Main.Left;
        left.Enable();


        control.Main.Left.Enable();

        right = control.Main.Right;
        right.Enable();


        control.Main.Right.Enable();

        spin = control.Main.Spin;
        spin.Enable();


        control.Main.Spin.Enable();

        instantFall = control.Main.InstantFall;
        instantFall.Enable();


        control.Main.InstantFall.Enable();
    }

    /// <summary>
    /// Resets the game state to play again after a game over.
    /// </summary>
    private void StartGame()
    {
        if (piece.isActiveAndEnabled)
        {
            return;
        }
        FreeAllBlocks();
        piece.gameObject.SetActive(true);
        piece.RestartPiece();
        
        piece.UpdateBlocks();
        
        
        Score = 0;
        Level = 0;
        GameOver = false;
    }

    //OnDisable is called when the object is deactivated or disabled.
    private void OnDisable()
    {
        DisableInputs();
    }

    /// <summary>
    /// Disables all of the control inputs.
    /// </summary>
    private void DisableInputs()
    {
        Debug.Log("Disabling inputs.");
        start.Disable();
        control.Main.Start.Disable();

        left.Disable();
        control.Main.Left.Disable();

        right.Disable();
        control.Main.Right.Disable();

        spin.Disable();
        control.Main.Spin.Disable();

        instantFall.Disable();
        control.Main.InstantFall.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //FreeAllBlocks();
        //Initialize the board.
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                Board[i,j] = null;
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameOver)
        {
            CheckForClearedRows();
        }

        if (Level > 28) GameSpeed = 1;
        else GameSpeed = gameSpeedByLevel[Level];

        SetBoardRotations();

        EndGame();


    }
    /// <summary>
    /// Makes the game over text inflate in the player's face when they lose.
    /// </summary>
    private void GameOverText()
    {
        if (!GameOver)
        {
            gameOverText.transform.localScale = new Vector3(0f, 0f, 0f);
        }
        else
        {
            if(gameOverText.transform.localScale.x < 1)
            {
                gameOverText.transform.localScale += new Vector3(1f, 1f, 1f)*Time.deltaTime;
            }
            else
            {
                gameOverText.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    /// <summary>
    /// Calls the methods associated with the game over state.
    /// </summary>
    private void EndGame()
    {
        GameOverText();
        GameOverDisplay();
    }

    /// <summary>
    /// Frees all blocks which are currently in use.
    /// </summary>
    public static void FreeAllBlocks()
    {
        for(int i = 0; i < 256; i++)
        {
            if (blockPool[i].isActiveAndEnabled)
            {
                poolIndex.Enqueue((byte)i); //Mark the index as containing a free block.
                RemoveBlock(blockPool[i]); //Remove the block from the play field and return it to the pool, in case it is in use.
            }
        }
        ClearBoard();
    }

    /// <summary>
    /// Sets every board tile to null.
    /// </summary>
    private static void ClearBoard()
    {
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                Board[i, j] = null;
            }
        }
    }

    /// <summary>
    /// Plays a little animation over the game over screen.
    /// </summary>
    private void GameOverDisplay()
    {
        if (GameOver)
        {
            gameOverDisplayTime += Time.deltaTime;
            if (gameOverDisplayTime > gameOverDisplayRate)
            {
                gameOverDisplayX++;
                if (gameOverDisplayX > 9)
                {
                    gameOverDisplayX = 0;
                    gameOverDisplayY++;
                }
                if (gameOverDisplayY > 19)
                {
                    gameOverDisplayY = 1;
                    if (!gameOverDisplayPhase) gameOverDisplayPhase = true;
                    else gameOverDisplayPhase = false;
                }
                if (!gameOverDisplayPhase)
                {
                    Board[gameOverDisplayX, gameOverDisplayY] = AddBlock();
                    Board[gameOverDisplayX, gameOverDisplayY].GridX = (byte)gameOverDisplayX;
                    Board[gameOverDisplayX, gameOverDisplayY].GridY = (byte)gameOverDisplayY;
                    Board[gameOverDisplayX, gameOverDisplayY].SetColor((Shape)Random.Range(0, 7));
                }
                else
                {
                    RemoveBlock(Board[gameOverDisplayX, gameOverDisplayY]);
                    Board[gameOverDisplayX, gameOverDisplayY] = null;
                }
                gameOverDisplayTime = 0f;
            }
        }
    }

    /// <summary>
    /// Handles the rotations of the transform of the board, so the board can rotate.
    /// </summary>
    private void SetBoardRotations()
    {
        Quaternion currentEulerAngles = BoardRotation;

        currentEulerAngles.eulerAngles += new Vector3(0, 0, 0) * Time.deltaTime;

        BoardRotation = currentEulerAngles;

        transform.rotation = BoardRotation;
    }

    /// <summary>
    /// Pulls a block out of the block pool for use.
    /// </summary>
    /// <returns>The block object that was pulled from the pool.</returns>
    public static Block AddBlock()
    {
        Block checkBlock;
        
        while (true)
        {
            checkBlock = blockPool[poolIndex.Dequeue()];
            if (!checkBlock.isActiveAndEnabled)
            {
                checkBlock.gameObject.SetActive(true);
                return checkBlock;
            }
        }
    }

    /// <summary>
    /// Returns a block object to the pool by deactivating it. Note that this method DOES NOT set the given reference to null, you need to do that yourself.
    /// </summary>
    /// <param name="b">The block object to return to the pool.</param>
    public static void RemoveBlock(Block b)
    {
        if (b != null)
        {
            b.GridX = 255;
            b.GridY = 255;
            b.gameObject.transform.position = new Vector3(1000000f, 1000000f, 1000000f); //Send the block someplace far away so it can't be seen on the frame where it is grabbed again.
            poolIndex.Enqueue(b.Index);
            b.gameObject.SetActive(false); //Deactivate the block.
        }
    }

    /// <summary>
    /// Sweeps over the board to find rows that the player successfully completed.
    /// </summary>
    private void CheckForClearedRows()
    {
        int numberOfRowsCleared = 0;
        int baseScore = 0;
        for(int i = 1; i < 20; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                if (Board[j, i] == null)
                {
                    break;
                }
                else
                {
                    if (!Board[j, i].isActiveAndEnabled) Debug.LogWarning($"Inactive block found on board at {i}, {j}.");
                    if (j == 9)
                    {
                        numberOfRowsCleared++;
                        TotalLinesCleared++;
                        if (TotalLinesCleared > 0 && TotalLinesCleared % 10 == 0) Level++;
                        ClearRow(i);
                        i--;
                    }
                }
            }
        }
        switch (numberOfRowsCleared)
        {
            case 0: { break;}
            case 1:
                {
                    baseScore = 40;
                    break;
                }
            case 2:
                {
                    baseScore = 100;
                    break;
                }
            case 3:
                {
                    baseScore = 300;
                    break;
                }
            case 4:
                {
                    baseScore = 1200;
                    break;
                }
        }
        Score += baseScore * (Level+1);
    }

    /// <summary>
    /// Clears the given row, as in when the player completed it.
    /// </summary>
    /// <param name="row">The row index to clear.</param>
    private void ClearRow(int row)
    {
        for(int i = 0; i < 10; i++)
        {
            
            Board[i, row].Clear = true;
            Board[i, row] = null;
        }

        DropRow(row);       
    }

    /// <summary>
    /// Lower all of the rows from the starting row up by one row.
    /// </summary>
    /// <param name="startingRow">The row that has been cleared.</param>
    private void DropRow(int startingRow)
    {
        for (int i = startingRow + 1; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (Board[j, i] != null)
                {
                    Color replaceColor = Board[j, i].gameObject.GetComponent<Renderer>().material.color;
                    RemoveBlock(Board[j, i]);

                    Board[j, i] = null;

                    Block newBlock = AddBlock();
                    Board[j, i - 1] = newBlock;
                    Board[j, i - 1].GridY = (byte)(i - 1);
                    Board[j, i - 1].GridX = (byte)j;
                    newBlock.SetColor(replaceColor);
                }
            }
        }
    }

    /// <summary>
    /// Alters the game state to be in the game over state.
    /// </summary>
    public void GameOverCondition()
    {
        GameOver = true;
        FreeAllBlocks();
        DisableInputs();
        piece.gameObject.SetActive(false);
    }

    
    //OnGUI is called for rendering and handling GUI events.
    void OnGUI()
    {
        int toNextLevel = 10 - (TotalLinesCleared % 10);

        GUI.Label(new Rect(16, 16, 100, 20), $"Score: {Score}");
        GUI.Label(new Rect(16, 32, 100, 20), $"Level: {Level}");
        GUI.Label(new Rect(16, 48, 100, 20), $"Next: {toNextLevel}");
        GUI.Label(new Rect(16, 64, 100, 20), $"FPS: {1 / Time.unscaledDeltaTime}");
    }
}
