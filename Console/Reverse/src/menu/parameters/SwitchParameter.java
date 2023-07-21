package menu.parameters;

//класс параметр, который может быть вкл или выкл
public class SwitchParameter extends DynamicParameter {

    public SwitchParameter(String _name, boolean position) {
        super(_name, new String[] {"выкл", "вкл"});
        value = position? values[1]: values[0];
    }

    public void selectValue() {
        value = value.equals(values[0])? values[1]: values[0];
    }
}
