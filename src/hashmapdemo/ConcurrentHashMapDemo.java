package hashmapdemo;
// source: https://www.geeksforgeeks.org/difference-hashmap-concurrenthashmap/
// source: https://www.geeksforgeeks.org/concurrenthashmap-in-java/

//Java program to illustrate 
//HashMap drawbacks 
import java.util.HashMap;
import java.util.concurrent.*;

public class ConcurrentHashMapDemo extends Thread {
	static ConcurrentHashMap<Integer, String> l = new ConcurrentHashMap<Integer, String>();

	public void run() {

		// Child add new element in the object
		l.put(103, "D");
		l.replace(100, "AA");
		try {
			Thread.sleep(1000);

			l.replace(101, "BB");
			Thread.sleep(500);
			l.replace(100, "AAA");

		} catch (InterruptedException e) {
			System.out.println("Child Thread going to add element");
		}
	}

	public static void main(String[] args) throws InterruptedException {
		l.put(100, "A");
		l.put(101, "B");
		l.put(102, "C");
		ConcurrentHashMapDemo t = new ConcurrentHashMapDemo();
		t.start();

		for (Object o : l.entrySet()) {
			Object s = o;
			System.out.println(s);
			Thread.sleep(1000);
		}
		System.out.println(l);
	}
}
