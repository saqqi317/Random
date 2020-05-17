package hotelSystem;

import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.Semaphore;

public class Room 
{
	public int number;
	
	// For Mapping the bookingRef with the days booked for this room.
	Map<String,HashSet<Integer>> bookingRefDaysMapper = new ConcurrentHashMap<>();
	
	// Semaphore lock = new Semaphore(1);
	
	public Room(int num) {
		number = num;		
	}		
	
	/**
	 * Books this room for given array of days with booking reference.
	 * @param bookingRef The reference booking.
	 * @param days Array of days to book this room.
	 */
	public void book(String bookingRef, Integer[] days)	{		
			
			// if the booking reference already exists then we should update it, 
	        // if its a new one then we should add.
	        // it is safe to assume that there is no conflict for the days that are being passed in 
	        // because the Hotel class already checks them.			
	        if (!bookingRefDaysMapper.containsKey(bookingRef)) {
	            // add new booking.
	        	bookingRefDaysMapper.put(bookingRef, new HashSet<Integer>(Arrays.asList(days)));	            	        	
	        }
	        else {
	            // update booking reference.	            
	        	bookingRefDaysMapper.replace(bookingRef, new HashSet<Integer>(Arrays.asList(days)));	        	        	
	        }
	}
	
	/**
	 * Gets all the days that are booked for this room regardless of the booking reference
	 * @return The array containing the days that are booked for this room
	 */
	public Set<Integer> getAllBookedDays() {
		Set<Integer> daysBooked = new HashSet<Integer>();
		
		// loop through all the bookingRefs that contain array of days of booking.
		for(HashSet<Integer> days : bookingRefDaysMapper.values()) {
			//  add each day of booking into the array to return.
			for(int day : days) {
				daysBooked.add(day);
			}
		}
				
		return daysBooked;
	}
	
	/**
	 * Gets all the days that are booked for this room except for the given booking reference.
	 * @param skipBookingRef The reference to skip.
	 * @return The array containing the days that are booked for this room except the given bookingRef
	 */
	public Set<Integer> getAllBookedDays(String skipBookingRef)	{
		Set<Integer> daysBooked = new HashSet<Integer>();
		
		for(String key : bookingRefDaysMapper.keySet())	{
			// skip the given booking reference and only return those booked for other booking references.
			if(key != skipBookingRef) {
				for(int day : bookingRefDaysMapper.get(key)) {
					daysBooked.add(day);
				}
			}
		}
		return daysBooked;	
	}
	
	/**
	 * Removes the booking days for given reference
	 * @param bookingRef The reference to remove
	 * @throws NoSuchBookingException
	 */
	public void cancelBooking(String bookingRef) throws NoSuchBookingException	{
			
			// if we already have a reference then remove it.
	        if (bookingRefDaysMapper.containsKey(bookingRef)) {
	            bookingRefDaysMapper.remove(bookingRef);	            
	        }
	        else {	        	
	        	// throw NoSuchBookingException if the reference is invalid or already removed / deleted.
	        	throw new NoSuchBookingException(bookingRef);
	        }
	}
}