using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

[RequireComponent(typeof(Text))]
public class ScriptBoard : MonoBehaviour {

    //клетки
    private ScriptCellPick[] _cellsPick;       //массив клеток выбора
    private ScriptCell[][] _cells;       //матрица клеток
    private ScriptCell _selectedCell;
    
    //цвета
    private Color _colorCellDefault;
    private Color _colorCellAnalog;
    private Color _colorCellSelected;
    private Color _colorSymbolRight;
    private Color _colorSymbolWrong;

    //клетка
    private String keyString = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    
    private int _size, _lSize, _dif;
    private float _cellSize;
    private bool _keyPressed;

    //элементы UI
    private Canvas _canvas;
    private Text _emptyText;
    private Text _mistakeText;
    
    public Text GetEmptyText()
    {
        return _emptyText;
    }

    public Canvas GetCanvas()
    {
        return _canvas;
    }

    public ScriptCell GetSelectedCell()
    {
        return _selectedCell;
    }

    // Start is called before the first frame update
    void Start()
    {
        _emptyText = GameObject.Find("Text").GetComponent<Text>();
        var cell = GameObject.Find("Cell");
        var cellPick = GameObject.Find("CellPick");
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        
        _keyPressed = false;
        SetLSize(3);
        _cellSize = 1.28f;
        _dif = 2;
        
        _emptyText.fontSize = (int)(_cellSize*100/1.72f);
        
        //Количество ошибок
        _mistakeText = Instantiate(
            _emptyText,
            new Vector3(7, 6, 0),
            Quaternion.identity,
            _canvas.transform);
        _mistakeText.fontSize = (int)(_cellSize * 33);
        _mistakeText.rectTransform.sizeDelta = new Vector2(_cellSize*200, _cellSize*100);
        _mistakeText.text = "Ошибок: 0";

        _selectedCell = null;
        _cells = new ScriptCell[_size][];
        _cellsPick = new ScriptCellPick[_size];
        var tr = transform;
        
        //игровое поле
        for (int i = 0; i < _size; ++i) {
            _cells[i] = new ScriptCell[_size];
            
            for (int j = 0; j < _size; ++j) {
                _cells[i][j] = Instantiate(cell).GetComponent<ScriptCell>();
                Vector3 newPosition = new Vector3(_cellSize * j, -_cellSize * i, 0.0f);
                Vector3 shift = new Vector3(-_cellSize*_size/2, _cellSize*_size/2);
                _cells[i][j].transform.SetPositionAndRotation(tr.position+ shift + newPosition, tr.rotation);
                _cells[i][j].SetBoard(gameObject, i, j);
                _cells[i][j].SetText(Instantiate(_emptyText, _canvas.transform));
            }
        }
        
        //клетки для заполнения
        for (int i = 0; i < _size; ++i)
        {
            _cellsPick[i] = Instantiate(cellPick).GetComponent<ScriptCellPick>();
            Vector3 newPosition = new Vector3(_cellSize * i, -_cellSize * (0.5f+_size), 0.0f);
            Vector3 shift = new Vector3(-_cellSize*_size/2, _cellSize*_size/2);
            _cellsPick[i].transform.SetPositionAndRotation(tr.position+ shift + newPosition, tr.rotation);
            _cellsPick[i].SetBoard(this, keyString[i].ToString());
        }
        
        
        Colorize(Color.Lerp(Color.grey, Color.white, 0.4f), Color.Lerp(Color.blue, Color.cyan, 0.7f));
        Fill();
    }

    // Update is called once per frame
    void Update()
    {
        if (_selectedCell == null || _selectedCell.IsOpen())
        {
            return;
        }
        
        if (!_keyPressed)
        {
            for (int i = 0; i < _size; ++i)
            {
                String str = keyString.Substring(i, 1).ToLower();
                if (Input.GetKey(str))
                {
                    SetValue(str.ToUpper());
                    _keyPressed = true;
                    return;
                }
            }
        } else if (!Input.anyKey)
        {
            _keyPressed = false;
        }
    }

    private void SetLSize(int newLSize)
    {
        _lSize = newLSize;
        _size = _lSize * _lSize;
    }

    private void OpenSelectedCell()
    {
        _selectedCell.SetOpen(true);
        _selectedCell.GetText().color = _colorSymbolRight;
        CheckPick();
    }

    //Метод проверяет все клетки выбора на полноту
    private void CheckPick()
    {
        var countValue = new int[_size];
        for (var i = 0; i < _size; ++i)
        {
            countValue[i] = 0;
        }
        
        for (var i = 0; i < _size; ++i)
        {
            for (var j = 0; j < _size; ++j)
            {
                if (_cells[i][j].IsOpen())
                {
                    var keyIndex = keyString.IndexOf(_cells[i][j].GetTrueValue()[0]);
                    ++countValue[keyIndex];
                }
                else if (!string.IsNullOrEmpty(_cells[i][j].GetUserValue()))
                {
                    var keyIndex = keyString.IndexOf(_cells[i][j].GetUserValue()[0]);
                    countValue[keyIndex] = -1;
                }
            }
        }
        
        for (int i = 0; i < _size; ++i)
        {
            if (countValue[i] == _size)
            {
                _cellsPick[i].SetVisible(false);
            }
        }
        //Debug.Log(countValue.ToString());
    }
    
    private void DoMistake()
    {
        _mistakeText.text = "Ошибок: " + Convert.ToString(Convert.ToInt32(_mistakeText.text.Substring(8))+1);
    }
    
    public void SetValue(String str)
    {
        if (_selectedCell == null) { return;}
        
        //отмена ошибки
        if (str.Equals(_selectedCell.GetUserValue()))
        {
            _selectedCell.SetUserValue("");
            CheckPick();
            return;
        }
        
        //
        if (_selectedCell.GetTrueValue().Equals(str))
        {
            OpenSelectedCell();
            MatchOpen(str);
        }
        else
        {
            _selectedCell.SetUserValue(str);
            _selectedCell.GetText().color = _colorSymbolWrong;
            DoMistake();
        }

        //SelectCell(_selectedCell.GetPosition());
    }

    void Trans()
    {
        for (int i = 0; i < _size; ++i)
        {
            for (int j = i+1; j < _size; ++j)
            {
                _cells[i][j].Swap(_cells[j][i]);
            }
        }
    }

    void SwapStrCol(int i1, int i2, bool swapStr)
    {
        if (i1 == i2) { return;}
        
        if (swapStr)
        {
            for (int k = 0; k < _size; ++k)
            {
                _cells[i1][k].Swap(_cells[i2][k]);
            }
        }
        else
        {
            for (int k = 0; k < _size; ++k)
            {
                _cells[k][i1].Swap(_cells[k][i2]);
            }
        }
    }
    
    void SwapAreaStrCol(int a1, int a2, bool swapStr)
    {
        if (a1 == a2) { return;}

        for (int i = 0; i < _lSize; ++i)
        {
            SwapStrCol(a1*_lSize + i, a2*_lSize + i, swapStr);
        }
    }
    
    void Fill()
    {
        var random = new Random();

        for (int i = 0; i < _size; ++i) {
            for (int j = 0; j < _size; ++j) {

                int index = (i/_lSize + (i%_lSize)*_lSize + j) % _size;
                
                _cells[i][j].SetTrueValue(keyString[index].ToString());
            }
        }

        for (int i = 0; i < _size*(_lSize+_dif); ++i)
        {
            int rx = random.Next(0, _size - 1);
            int ry = random.Next(0, _size - 1);
            _cells[ry][rx].SetOpen(true);
        }
        Randomize(random);
        Redraw();
    }

    private void Redraw()
    {
        for (int i = 0; i < _size; ++i)
        {
            for (int j = 0; j < _size; ++j)
            {
                _cells[i][j].DrawText();
            }
        }
    }
    
    void Randomize(Random random)
    {
        for (int i = 0; i < 100; ++i)
        {
            int ch = random.Next(1, 4);
            switch (ch)
            {
                case 1:
                    Trans();
                    break;
                
                case 2:
                    int
                        a = random.Next(0, _lSize),
                        i1 = random.Next(0, _lSize),
                        i2 = random.Next(0, _lSize);
                    SwapStrCol(a*_lSize+i1, a*_lSize+i2, random.Next()%2 == 0);
                    break;
                case 3:
                    int
                        a1 = random.Next(0, _lSize),
                        a2 = random.Next(0, _lSize);
                    SwapAreaStrCol(a1, a2, random.Next()%2 == 0);
                    break;
            }
        }
    }

    public void SelectCell(Vector2Int newPosition)
    {
        var oldSelectedCell = _selectedCell;

        _selectedCell = _cells[newPosition.x][newPosition.y];

        if (oldSelectedCell == _selectedCell) { return;}
        
        if (_selectedCell.IsOpen())
        {
            MatchOpen(_selectedCell.GetTrueValue());
        } 
        else
        {
            _selectedCell.SetCellColor(_colorCellSelected);
            if (oldSelectedCell != null && !oldSelectedCell.IsOpen())
            {
                oldSelectedCell.ResetCellColor();
            }
        }

    }

    //Функция показывает открытые клетки с конкретным значением
    public void MatchOpen(String key)
    {
        if (string.IsNullOrEmpty(key)) { return; }

        var keyIndex = keyString.IndexOf(key[0]);

        for (int i = 0; i < _size; ++i)
        {
            //сначала определяем цвет клетки для выбора
            _cellsPick[i].GetComponent<SpriteRenderer>().color = i==keyIndex? _colorCellSelected: _colorCellDefault;
            
            for (int j = 0; j < _size; ++j)
            {
                var scrij = _cells[i][j].GetComponent<ScriptCell>();
                if (scrij.IsOpen() && scrij.GetTrueValue().Equals(key))
                {
                    scrij.SetCellColor(_colorCellSelected);
                }
                else
                {
                    scrij.ResetCellColor(); 
                }
            }
        }
    }

    void Colorize(Color color1, Color color2) {
        _colorCellDefault = color1;
        _colorCellDefault.a = 0.84f;
        _colorCellAnalog = Color.Lerp(_colorCellDefault, Color.black, 0.4f);
        _colorCellSelected = Color.Lerp(_colorCellAnalog, color2, 0.7f);
        _colorSymbolRight = Color.Lerp(_colorCellDefault, Color.white, 0.8f);
        _colorSymbolWrong = Color.Lerp(_colorSymbolRight, Color.red, 0.8f);
                    
        for (byte i = 0; i < _size; ++i)
        {
            var rendPick = _cellsPick[i].gameObject.GetComponent<SpriteRenderer>();
            rendPick.color = _colorCellDefault;
            for (byte j = 0; j < _size; ++j) {
                var rend = _cells[i][j].gameObject.GetComponent<SpriteRenderer>();
                rend.color = (i/_lSize + j/_lSize)%2 == 0? _colorCellDefault: _colorCellAnalog;
                _cells[i][j].GetText().color = _colorSymbolRight;
                _cells[i][j].SetCellDefaultColor(rend.color);
            }
        }

        if (_selectedCell != null) {
            _selectedCell.gameObject.GetComponent<SpriteRenderer>().color = _colorCellSelected;
        }
    }
    
}
