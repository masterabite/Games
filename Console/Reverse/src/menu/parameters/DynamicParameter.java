package menu.parameters;

//абстрактный класс родитель для параметров, которые могут быть изменены пользователем
public abstract class DynamicParameter extends Parameter {
    protected String[] values;

    public DynamicParameter(String _name, String _value) {
        super(_name, _value);
    }

    public DynamicParameter(String _name, String[] _values) {
        super(_name, _values[0]==null? "-": _values[0]);
        values = _values;
    }

    @Override
    public String toString() {
        return name + ": " + value;
    }

    public abstract void selectValue();
}
