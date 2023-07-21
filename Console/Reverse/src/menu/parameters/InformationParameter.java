package menu.parameters;

public class InformationParameter extends Parameter {
    public InformationParameter(String _name, String _value) {
        super(_name, _value);
    }

    @Override
    public String toString() {
        return name + ": " + value;
    }
}
