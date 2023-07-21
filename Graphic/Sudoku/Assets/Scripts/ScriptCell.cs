using System;
using UnityEngine;
using UnityEngine.UI;

public class ScriptCell : MonoBehaviour
{

    private GameObject _board;
    private String _trueValue;
    private String _userValue;
    private String _drawValue;
    private Color _cellDefaultColor;
    private Vector2Int _position;
    private bool _open;
    private Text _myText;

    public bool IsOpen()
    {
        return _open;
    }
    public void SetOpen(bool open)
    {
        this._open = open;
        DrawText();
    }

    public Vector2Int GetPosition()
    {
        return _position;
    }
    
    public void SetText(Text text)
    {
        text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        _myText = text;
    }

    public Text GetText()
    {
        return _myText;
    }
    
    public void DrawText()
    {
        _drawValue = _open? _trueValue: _userValue;
        if (_myText != null)
        {
            _myText.text = _drawValue;
        }
    }

    public void Swap(ScriptCell other)
    {
        (_trueValue, other._trueValue) = (other._trueValue, _trueValue);
    }
    
    public void SetBoard(GameObject board, int i, int j)
    {
        _position = new Vector2Int(i, j);
        this._board = board;
    }

    public void SetCellDefaultColor(Color color)
    {
        _cellDefaultColor = color;
    }

    public void SetCellColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ResetCellColor()
    {
        GetComponent<SpriteRenderer>().color = _cellDefaultColor;
    }
    
    public void SetTrueValue(String value)
    {
        _trueValue = value;
        DrawText();
    }

    public void SetUserValue(String value) { _userValue = value; DrawText();}

    public String GetTrueValue() { return _trueValue; }
    public String GetUserValue() { return _userValue; }

    public void OnMouseEnter()
    {
        if (Input.GetMouseButton(0)) {
            _board.GetComponent<ScriptBoard>().SelectCell(_position);
        }
    }
    
    public void OnMouseDown()
    {
        _board.GetComponent<ScriptBoard>().SelectCell(_position);
    }

}
