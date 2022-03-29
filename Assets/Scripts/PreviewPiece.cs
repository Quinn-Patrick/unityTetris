using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPiece : Piece
{
    private Shape currentShape;

    

    // Start is called before the first frame update
        void Start()
    {
        TopRightX = 15;
        TopRightY = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentShape != NextShape) //The preview piece should only be updated if it is currently the wrong piece.
        {
            base.ErasePiece();
            base.OrientPiece(Piece.NextShape);
            currentShape = NextShape;
            UpdateBlocks();
            SetPreviewPieceColor();
        }
    }

    /// <summary>
    /// Set the color of the preview piece.
    /// </summary>
    private void SetPreviewPieceColor()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (LocalGrid[i, j] != null)
                {
                    LocalGrid[i, j].SetColor(Piece.NextShape);
                }
            }
        }
    }
}
