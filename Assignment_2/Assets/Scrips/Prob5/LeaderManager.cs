using System.Runtime.CompilerServices;
using System.Threading;

public class LeaderManager {
    private static LeaderManager instance = null;
    int currentLeader;
    int currentIndex;

    public static LeaderManager getInstance() {
        if (instance == null) {
            instance = new LeaderManager();
        }

        return instance;
    }

    public int leader {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get { return currentLeader; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set { currentLeader = value; }
    }



    public int getID() {
        return Interlocked.Increment(ref currentIndex);
    }

    private LeaderManager() {
        currentLeader = 1;
        currentIndex = 0;
    }

}