using System;
using UnityEngine;
using UnityEngine.UI;

public class ScriptCellPick : MonoBehaviour
{

    private ScriptBoard _board;
    private Text _myText;
    
    public void SetBoard(ScriptBoard board, String key)
    {
        this._board = board;
        if (_myText == null)
        {
            _myText = Instantiate(this._board.GetEmptyText(),transform.position, Quaternion.identity, this._board.GetCanvas().transform);
        } 
        
        _myText.text = key;
    }

    public void OnMouseUp()
    {
        if (_board != null)
        {
            var selectedCell = _board.GetSelectedCell();
            if (selectedCell != null && !selectedCell.IsOpen())
            {
                _board.GetComponent<ScriptBoard>().SetValue(_myText.text);
            }
            else
            {
                _board.MatchOpen(_myText.text);
            }
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        _myText.gameObject.SetActive(visible);
    }
}
