using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper : MonoBehaviour
{

    public enum Type : int
    {
        Unsetted, // If this is true, then set manually to default
        //FixedPoint = 0, // This can be done following a non-moving GameObject
        //SnapToPoints, // This has different FixedPoints, and snaps to the one closer to the player
        FixedAxis,
        FixedAxisWithOffset,
        Follow,
        FollowWithOffset
    }

    #region Variables
    // Main settings
    [Header( "Main settings" ), SerializeField] private Type type;
    new private Transform camera;
    public Transform target;// { private set; get; }

    // Fixed axis specific variables
    [Header( "Fixed axis" )]
    public bool isHorizontal = true; // True = horizontal. False = vertical
    public float fixedValue = 0f; // If the axis is horizontal, this is the height. If the axis is vertical, this is the X value

    [Header( "With offset" )]
    public float offsetValue = 0.2f;


    // Follow type specific variables


    // Cursor locking
    /*
    [SerializeField]
    private bool cursorLocking = true;
    private CursorLocking cursorLock;
    */

    // Movement settings
    /*
    [SerializeField,Header("Fidgeting")]
    private float damping = 5.0f;
    private bool smoothRotation = false;
    private float rotationDamping = 10.0f;
    */

    #endregion

    #region Monobehaviour
    private void Awake()
    {
        camera = transform;

    }

    void FixedUpdate()
    {
        if (camera != null)
        {
            if (target != null)
            {
                switch (type)
                {
                    case Type.FixedAxis:
                        CameraFixedAxis();
                        break;

                    case Type.FixedAxisWithOffset:
                        CameraFixedAxisWithOffset();
                        break;

                    case Type.Follow:
                        CameraFollow();
                        break;

                    case Type.FollowWithOffset:
                        CameraFollowOffset();
                        break;

                }
            }
        }
        else
        {
            //Debug.LogError( "There's no Camera component" + name );
        }
    }
    #endregion


    #region Options

    private void CameraFixedAxis()
    {
        Vector3 newPos = Vector3.zero;
        if (target != null)
        {
            float newPosX = (isHorizontal) ? target.localPosition.x : fixedValue;
            float newPosY = (isHorizontal) ? fixedValue : target.localPosition.y;
            newPos = new Vector3( newPosX, newPosY, -10 );
            camera.localPosition = newPos;
        }
        else
        {
            //Debug.LogError( "There's no target in camera " + name );
        }
    }

    private void CameraFixedAxisWithOffset()
    {
        Vector3 newPos = Vector3.zero;
        if (target != null)
        {
            float newPosX = (isHorizontal) ? target.localPosition.x + offsetValue : fixedValue;
            float newPosY = (isHorizontal) ? fixedValue : target.localPosition.y + offsetValue;
            newPos = new Vector3( newPosX, newPosY, -10 );

            // Apply the offset frontfacing the target direction
            //Vector3 offset = target.transform.up.normalized * offsetValue;
            //newPos = newPos + offset;
            //newPos.z = -10;

            camera.localPosition = newPos;
        }
        else
        {
            //Debug.LogError( "There's no target in camera " + name );
        }
    }

    private void CameraFollow()
    {
        Vector3 newPos = Vector3.zero;
        if (target != null)
        {
            newPos = new Vector3( target.localPosition.x, target.localPosition.y, -10 );
            camera.localPosition = newPos;
        }
        else
        {
            //Debug.LogError( "There's no target in camera " + name );
        }
    }

    private void CameraFollowOffset()
    {
        Vector3 newPos = Vector3.zero;
        if (target != null)
        {
            newPos = new Vector3( target.localPosition.x, target.localPosition.y, -10 );

            // Apply the offset frontfacing the target direction
            Vector3 offset = target.transform.up.normalized * offsetValue;
            newPos = newPos + offset;
            newPos.z = -10;

            camera.localPosition = newPos;
        }
        else
        {
            //Debug.LogError( "There's no target in camera " + name );
        }
    }
    #endregion


    #region Public
    public void Set( Type typeP = Type.Follow, Transform targetP = null )
    {
        type = typeP;
        target = targetP;
    }

    public void Set( Type typeP = Type.FixedAxis, Transform targetP = null, bool isHorizontalP = false, float fixedValueP = 0f )
    {
        type = typeP;
        target = targetP;
        isHorizontal = isHorizontalP;
        fixedValue = fixedValueP;
    }

    public void Set( Type typeP = Type.FixedAxisWithOffset, Transform targetP = null, bool isHorizontalP = false, float fixedValueP = 0f, float offsetValueP = 0.2f )
    {
        type = typeP;
        target = targetP;
        isHorizontal = isHorizontalP;
        fixedValue = fixedValueP;
        offsetValue = offsetValueP;
    }

    public void Set( Type typeP = Type.FollowWithOffset, Transform targetP = null, float offsetValueP = 0.2f )
    {
        type = typeP;
        target = targetP;
        offsetValue = offsetValueP;
    }
    #endregion
}
