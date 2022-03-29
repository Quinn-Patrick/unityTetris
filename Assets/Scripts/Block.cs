using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Block : MonoBehaviour
{
    //The X and Y positions of the block.
    public byte GridX { get; set; } = 255;
    public byte GridY { get; set; } = 255;

    //The particle effect that occurs when the block is cleared from a row.
    public Object effect;

    //The amount of time that the block lingers invisibly so that the particles can play.
    private float clearTimer = 5.0f;
    //The maximum amount of time that the block can linger invisibly so that the particles can play.
    private readonly float clearTimerMax = 5.0f;

    //Whether or not the block has been cleared, but hasn't yet been deactivated.
    public bool Clear { get; set; } = false;

    //The material of the block.
    Material material;

    //The index in the block pool of this block.
    public byte Index { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VisualEffect>().Stop();
        
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float eulerBoardYAngle = Game.BoardRotation.eulerAngles.y*Mathf.Deg2Rad;       
        gameObject.transform.SetPositionAndRotation(new Vector3(((float)GridX - 4.5f)*-Mathf.Cos(eulerBoardYAngle), GridY, ((float)GridX - 4.5f) * Mathf.Sin(eulerBoardYAngle)), Game.BoardRotation);
        ClearCountdown();
    }

    /// <summary>
    /// Places the block on the game board according to gridX and gridY.
    /// </summary>
    public void Lock()
    {
        if (GridY > 19) return;

        Game.Board[GridX, GridY] = this;
        GetComponent<VisualEffect>().Play();

    }

    //OnDisable is called when the object becomes inactive.
    public void OnDisable()
    {
        GetComponent<VisualEffect>().Stop();
    }

    /// <summary>
    /// Allows the block to linger in the scene while the particles play after it was part of a cleared row.
    /// </summary>
    private void ClearCountdown()
    {
        if (Clear)
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            if(Mathf.Abs(clearTimerMax - clearTimer) < 0.01)gameObject.GetComponent<VisualEffect>().Play();
            else GetComponent<VisualEffect>().Stop();

            clearTimer -= Time.deltaTime;
            if (clearTimer < 0)
            {
                clearTimer = clearTimerMax;
                Clear = false;
                gameObject.GetComponent<Renderer>().enabled = true;
                Game.RemoveBlock(this);
            }
        }
        else GetComponent<VisualEffect>().Stop();
    }

    /// <summary>
    /// Sets the color of the block.
    /// </summary>
    /// <param name="shape">The shape of the piece that the block is part of.</param>
    public void SetColor(Shape shape)
    {
        material = GetComponent<Renderer>().material;
        switch (shape)
        {
            case Shape.Line:
                {
                    material.color = new Color(0.3098f, 0.388235f, 0.40392f);
                    break;
                }
            case Shape.L_Left:
                {
                    material.color = new Color(0.933333f, 0.96078f, 0.858823f);
                    break;
                }
            case Shape.L_Right:
                {
                    material.color = new Color(0.996078f, 0.372549f, 1/3f);
                    break;
                }
            case Shape.T:
                {
                    material.color = new Color(1f, 0.5490196f, 0.7764706f);
                    break;
                }
            case Shape.S_Left:
                {
                    material.color = new Color(0.443137f, 0.96863f, 0.623529f);
                    break;
                }
            case Shape.S_Right:
                {
                    material.color = new Color(0.6f, 0.7607843f, 0.635294f);
                    break;
                }
            case Shape.Box:
                {
                    material.color = new Color(0.04902f, 0.3568627f, 0.6509804f);
                    break;
                }
        }
    }

    /// <summary>
    /// Sets the color of the block. Overloaded.
    /// </summary>
    /// <param name="color">The exact color to set the block to.</param>
    public void SetColor(Color color)
    {
        material = GetComponent<Renderer>().material;
        material.color = color;
    }

    //OnDestroy is called when MonoBehavior is destroyed.
    void OnDestroy()
    {
        //Destroy the material variable.
        Destroy(material);
    }

}
