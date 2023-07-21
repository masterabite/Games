package exceptions;

public class BoardOutException extends Exception {
    public BoardOutException() {
        super("выход за размер доски");
    }
}
