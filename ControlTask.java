public class ControlTask implements Runnable {

    private volatile boolean[] stopFlags;
    private final int[] timeDelay;

    public ControlTask(boolean[] stopFlags, int[] timeDelay) {
        this.stopFlags = stopFlags;
        this.timeDelay = timeDelay;
    }

    @Override
    public void run() {
        int n = stopFlags.length;
        boolean[] stopped = new boolean[n];
        int stoppedCount = 0;

        long startTime = System.currentTimeMillis();

        while (stoppedCount < n) {
            long elapsed = System.currentTimeMillis() - startTime;

            for (int i = 0; i < n; i++) {
                if (!stopped[i] && elapsed >= timeDelay[i]) {
                    stopFlags[i] = true;
                    stopped[i] = true;
                    stoppedCount++;

                    System.out.println("Контролер зупинив потік " + (i + 1) +
                            " (delay = " + timeDelay[i] + " мс)");
                }
            }
        }
    }
}