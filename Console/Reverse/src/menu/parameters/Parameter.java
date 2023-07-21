package menu.parameters;

//класс родитель для всех параметров
public abstract class Parameter {
    protected String name;          //имя параметра
    protected String value;         //значение параметра

    public Parameter(String _name, String _value) {
        name = _name;
        value = _value;
    }

    public String getName() {
        return name;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String newValue) {
        value = newValue;
    }

}
