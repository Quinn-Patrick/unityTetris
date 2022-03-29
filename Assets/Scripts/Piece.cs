
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Piece : MonoBehaviour
{
    

    //The grid representing the shape of the given piece.
    public Block[,] LocalGrid { get; set; } = new Block[5, 5];

    //The x position on the grid of the top right corner of the piece.
    public byte TopRightX { get; set; }

    //The y position on the grid of the top right corner of the piece.
    public byte TopRightY { get; set; }

    //The amount of time in seconds that has elapsed since the last time the block fell.
    private float dropTime = 0f;

    //The orientation of the block, can be 0, 1, 2, or 3.
    private byte rotation = 0;

    //The enum representing the shape of the piece.
    public static Shape Shape { get; private set; }

    //The rate at which the piece falls is mutliplied by this factor. It can be either 1 or 2 (if the piece is fast falling).
    public float IsFastFalling { get; set; } = 1f;

    //The shape of the piece that will fall after the current one.
    public static Shape NextShape { get; set; }

    public Game GameObject;

    

    

    

    

    // Start is called before the first frame update
    void Start()
    {
        NextShape = (Shape)Random.Range(0, 7);
        RestartPiece();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Game.GameOver)
        {
            UpdateBlocks();

            

            dropTime += Time.deltaTime;
            if (dropTime > (Game.GameSpeed/60) / IsFastFalling)
            {
                if (!CheckLanded())
                {
                    dropTime = 0;
                    TranslatePieceVertical(true, true);
                }
            }
        }
    }

    /// <summary>
    /// Checks whether or not any block in the piece has landed.
    /// </summary>
    /// <returns>True if the piece has landed, false otherwise.</returns>
    private bool CheckLanded()
    {
        for (byte i = 0; i < 5; i++)
        {
            for (byte j = 0; j < 5 ; j++)
            {
                if (LocalGrid[i, j] != null)
                {                    
                    if(LocalGrid[i, j].GridX < 10 && (LocalGrid[i, j].GridY == 1 || Game.Board[LocalGrid[i, j].GridX, LocalGrid[i, j].GridY - 1] != null))
                    {
                        LandPiece();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Locks all of the blocks composing the piece to the board in their current position and then restarts the piece.
    /// </summary>
    private void LandPiece()
    {
        for (byte i = 0; i < 5; i++)
        {
            for (byte j = 0; j < 5; j++)
            {
                if (LocalGrid[i, j] != null)
                {
                    if (LocalGrid[i, j].GridY > 19) this.gameObject.SetActive(false);

                    LocalGrid[i, j].Lock();                    
                }
            }
        }
        RestartPiece();
        UpdateBlocks();
        if (IllegalPosition())
        {
            GameObject.GameOverCondition();
        }
    }

    /// <summary>
    /// Turns the piece into a new shape and returns it to the top of the board.
    /// </summary>
    public void RestartPiece()
    {
        
        Shape = NextShape;
        NextShape = (Shape)Random.Range(0, 7);
        TopRightX = 7;
        TopRightY = 21;
        rotation = 0;
        OrientPiece(Shape);
        dropTime = -1f + Game.GameSpeed;
        UpdateBlocks();
        
    }

    /// <summary>
    /// Translate the piece in the horizontal direction by 1 block.
    /// </summary>
    /// <param name="dir">The direction to translate in. True = left, False = right.</param>
    /// <param name="safe">If this is true, the block will not translate into an illegal position.</param>
    /// <returns>Whether or not the piece ends the function in an illegal position. Always returns true when safe is true.</returns>
    public bool TranslatePieceHorizontal(bool dir, bool safe)
    {
        byte storeTopRightX = TopRightX;

        //true = left, false = right
        if (dir)
        {
            TopRightX--;
        }
        else
        {
            TopRightX++;
        }

        UpdateBlocks();


        if (IllegalPosition())
        {
            if (safe)
            {
                TopRightX = storeTopRightX;
                return true;
            }
            else return false;
        }
        else return true;       
    }

    /// <summary>
    /// Moves the piece vertically by one block.
    /// </summary>
    /// <param name="dir">The direction to translate. True for down, False for up.</param>
    /// <param name="safe">If this is true, the block will not translate into an illegal position.</param>
    /// <returns>Whether or not the piece ends the function in an illegal position. Always returns true when safe is true.</returns>
    public bool TranslatePieceVertical(bool dir, bool safe)
    {
        byte storeTopRightY = TopRightY;

        //true = DOWN, false = UP
        if (dir)
        {
            TopRightY--;
        }
        else
        {
            TopRightY++;
        }

        UpdateBlocks();


        if (IllegalPosition())
        {            
            if (safe)
            {
                TopRightY = storeTopRightY;
                if (!dir)
                {
                    LandPiece();
                }
                return true;
            }
            else return false;
        }
        else return true;
    }

    /// <summary>
    /// Checks whether or not the piece is in a position that overlaps with a block or with the walls.
    /// </summary>
    /// <returns>Returns true if the block is in an illegal position, returns false otherwise.</returns>
    public bool IllegalPosition()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (LocalGrid[i, j] != null)
                {
                    try
                    {
                        if (LocalGrid[i, j].GridX > 9 || LocalGrid[i, j].GridY < 1 || Game.Board[LocalGrid[i, j].GridX, LocalGrid[i, j].GridY] != null)
                        {
                            return true;
                        }
                    }catch(System.IndexOutOfRangeException e)
                    {
                        Debug.LogError(e.Message);
                        Debug.LogError($"Index out of range exception, i = {i}, j = {j}.");
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Sets the grid positions of the blocks composing the piece to their appropriate values.
    /// </summary>
    public void UpdateBlocks()
    {
        for (byte i = 0; i < 5; i++)
        {
            for (byte j = 0; j < 5; j++)
            {
                if (LocalGrid[i, j] != null)
                {
                    LocalGrid[i, j].GridX = (byte)(TopRightX - i);
                    LocalGrid[i, j].GridY = (byte)(TopRightY - j);
                    LocalGrid[i, j].SetColor(Shape);
                }
            }
        }
    }

    /// <summary>
    /// Applies a rotation to the piece.
    /// </summary>
    public void SpinPiece()
    {
        byte storeRotation = rotation;

        rotation++;
        if (rotation > 3) rotation = 0;

        ErasePiece();

        Debug.Log("Attempting Spin");

        OrientPiece(Shape);
        UpdateBlocks();

        //Push the block out of an illegal position, and bail on the rotation if it's too difficult.
        if (IllegalPosition())
        {
            byte storeTopRightX = TopRightX;
            byte storeTopRightY = TopRightY;

            bool exited;

            //Attempt a downward translation first.
            exited = TranslatePieceVertical(true, false);
            if (exited) return;
            exited = TranslatePieceVertical(true, false);
            if (exited) return;

            TopRightX = storeTopRightX;
            TopRightY = storeTopRightY;            

            //Try left and right next.
            exited = TranslatePieceHorizontal(false, false);
            if (exited) return;
            exited = TranslatePieceHorizontal(false, false);
            if (exited) return;

            TopRightX = storeTopRightX;
            TopRightY = storeTopRightY;

            exited = TranslatePieceHorizontal(true, false);
            if (exited) return;
            exited = TranslatePieceHorizontal(true, false);
            if (exited) return;

            TopRightX = storeTopRightX;
            TopRightY = storeTopRightY;

            //Attempt a downward translation only as a last resort.
            exited = TranslatePieceVertical(false, false);
            if (exited) return;
            exited = TranslatePieceVertical(false, false);
            if (exited) return;

            TopRightX = storeTopRightX;
            TopRightY = storeTopRightY;

            //If the function hasn't returned by now, bail on the rotation.
            rotation = storeRotation;

            ErasePiece();

            Debug.Log("Spin Failed");


            OrientPiece(Shape);
            UpdateBlocks();

        }
    }

    /// <summary>
    /// Clears the blocks in the grid and returns them to the block pool.
    /// </summary>
    public void ErasePiece()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (LocalGrid[i, j] != null)
                {
                    Game.RemoveBlock(LocalGrid[i, j]);
                    LocalGrid[i, j] = null;
                }
            }
        }
    }

    /// <summary>
    /// Checks the shape and rotation of a piece and sets up the grid accordingly.
    /// </summary>
    public void OrientPiece(Shape inputShape)
    {
        switch (inputShape)
        {
            case Shape.Line:
                {
                    switch (rotation) {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), Game.AddBlock() }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), Game.AddBlock() }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                    }
                    break;

                }
            case Shape.L_Left:
                {
                    switch (rotation)
                    {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, Game.AddBlock(), null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock() }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null } };
                                break;
                            }

                    }
                    break;
                }
            case Shape.L_Right:
                {
                    switch (rotation)
                    {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock() }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }                        
                    }
                    break;
                }
            case Shape.T:
                {
                    switch (rotation) {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), Game.AddBlock(), null }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                    }
                    break;
                }
            case Shape.S_Left:
                {
                    switch (rotation)
                    {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, Game.AddBlock(), null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, null, Game.AddBlock(), null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, Game.AddBlock(), null, null }, { null, null, null, null, null } };
                                break;
                            }
                    }
                    break;
                }
            case Shape.S_Right:
                {
                    switch (rotation)
                    {
                        case 0:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 1:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                        case 2:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, null, null, null }, { null, null, null, null, null } };
                                break;
                            }
                        case 3:
                            {
                                LocalGrid = new Block[,] { { null, null, null, null, null }, { null, null, Game.AddBlock(), null, null }, { null, null, Game.AddBlock(), Game.AddBlock(), null }, { null, null, null, Game.AddBlock(), null }, { null, null, null, null, null } };
                                break;
                            }
                    }
                    break;
                }
            case Shape.Box:
                {
                    LocalGrid = new Block[,] { { null, null, null, null, null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, Game.AddBlock(), Game.AddBlock(), null, null }, { null, null, null, null, null }, { null, null, null, null, null } };
                    break;
                }
        }
    }
}
