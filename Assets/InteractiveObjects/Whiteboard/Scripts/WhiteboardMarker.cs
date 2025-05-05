using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class WhiteboardMarker : MonoBehaviour
{
    public InputActionProperty drawButton;
    //[SerializeField] private Button drawButton;
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 5;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;

    private RaycastHit _touch;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    private Whiteboard _whiteboard;


    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        // create a 25 length array of color (5x5 square of pen color) 
        // TODO modify if circular marker is desired
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y;
        // TODO check, init whiteboard
        _whiteboard = null;
    }

    void Update()
    {
        // check whiteboard each frame and change texture at that point 
        Draw();
    }

    private void Draw()
    {

        // if (!drawButton.action.IsPressed()){
        //     return;
        // }

        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
        {
            // does touch object interact with whiteboard
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    // "cache" touch
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                // determine which pixels are being touched (float -> pixel) 
                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize/2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize/2));

                // top drawing when stop touch
                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return; 

                // start draw
                if (_touchedLastFrame)
                {
                    _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                    // increment in 0.01 INCREASE THE VAL +=0.01 FOR BETTER FRAME RATE, decrease for better lerp
                    for (float f = 0.01f; f < 1.00f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    // need to lock rotation of the pen at impact
                    transform.rotation = _lastTouchRot;
                    
                    // apply
                    _whiteboard.texture.Apply();
                }

                // update vals
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }
        // did not touch whiteboard
        _whiteboard = null;
        _touchedLastFrame = false;
    }

}
