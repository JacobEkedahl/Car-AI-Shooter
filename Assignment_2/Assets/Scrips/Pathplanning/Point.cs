public class Point {
    public int i, j;
    public Point(int i, int j) {
        this.i = i;
        this.j = j;
    }

    public override string ToString() {
        return "i:" + i + ";j:" + j;
    }
}