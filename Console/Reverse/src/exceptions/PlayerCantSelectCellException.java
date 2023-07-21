package exceptions;

public class PlayerCantSelectCellException extends Exception {
    public PlayerCantSelectCellException(String message) {
        super(String.format("Клетка не может быть выбрана: %s", message));
    }
}
