package hotelSystem;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.Semaphore;

public class Hotel 
{
		// Maps the room number with the room object.
		HashMap<Integer, Room> roomMap = new HashMap<Integer, Room>();
	    
		// Maps the reference with the list of the room numbers that booking reference represents.
	    HashMap<String, List<Integer>> bookingRefRoomNumMap = new HashMap<String, List<Integer>>();
	    
	    Semaphore lock = new Semaphore(1);	    	    
	    
	    /// <summary> Class Constructor Method </summary>
	    /// <param name="roomNums"> Array of rooms which Hotel contains</param>
	    public Hotel(int[] roomNums)
	    {
	    	for(int roomNumber : roomNums)
	    	{
	    		roomMap.put(roomNumber, new Room(roomNumber));
	    	}
	    }
	    
	    /// <summary> Checks if the room is already booked for given days array. </summary>
	    /// <param name="days">Array of days to check.</param>
	    /// <param name="roomNumber">Room number.</param>
	    /// <returns>A value indicating if (even for a single day in given array) room is already booked. True means its booked, false otherwise.</returns>
	    public boolean roomBooked(Integer[] days, int roomNumber)
	    {
	    	if(days == null || days.length == 0)
	    	{
	            // if no days are given then there is nothing to check.
	            // return early.
	    		return false;
	    	}
	    	
	    	// get the room object using given room number.
	    	// It is assumed that the passed roomNumber is valid
	    	//roomMap.get returns the value to which the specified key is mapped
	    	Room room = roomMap.get(roomNumber);
	    	
	    	// Get all the days that are booked for given room.
	    	Set<Integer> bookedDays = new HashSet<Integer>();    	
	    	bookedDays = room.getAllBookedDays();
	    	
	    	if(bookedDays.size() == 0)
	    	{
	    		// if no days are booked then there is not need to check for given days.
	            // we can return early from this method.
	            return false;
	    	}
	    	
	    	// loop through each day and check if we have a match. 	        
	        for (int day : days)
	        {
	            // bookedDays Hashset gives us constant time lookup.
	            if (bookedDays.contains(day))
	            {
	                return true;
	            }
	        }

	        // returning false means we do not find any booked room for given days.
	        return false;    	
	    }
	    
	    /// <summary> Books the room with given booking reference for the days for the room number. </summary>
	    /// <param name="bookingRef">The booking reference to keep track of days and room.</param>
	    /// <param name="days">Array containing the days to book.</param>
	    /// <param name="roomNum">The room number to book.</param>
	    /// <returns>A value indicating if the booking was successful. True means successful, false otherwise.</returns>
	    public boolean bookRoom(String bookingRef, Integer[] days, int roomNum)
	    {
	    	try 
	    	{								
				// using RoomBooked method to check if this room is available for given days.
		        if (!roomBooked(days, roomNum))
		        {
		        	// once we are here it means we have unbooked days for this room number.
		            // so booking this room is possible.

		            // get the room object using given room number.
		        	// It is assumed that the roomNumber is valid
		        	
		        	Room roomToBook = roomMap.get(roomNum);
		        	
		        	lock.acquire();
		        	
		        	// since we have already checked if this room has given days available 
		            // we can just call it to book.
		            roomToBook.book(bookingRef, days);
		            
		            // update bookingRefRoomNumMap mapper to keep track of booking reference with the rooms.
		            // we need list of rooms because in Cancel booking method, we only have the booking reference 
		            List<Integer> roomNumbers = new ArrayList<Integer>();
		            roomNumbers.add(roomNum);
		            bookingRefRoomNumMap.put(bookingRef, roomNumbers);
		            
		            lock.release();
		            return true;        	
		        }
			} 
	    	catch (InterruptedException e) 
	    	{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	
	    	lock.release();
	        // returning false means we could not find the vacant room for given array of days.
	        return false;
	    }
	    
	    
	    
	    /// <summary> Updates the existing booking.  </summary>
	    /// <param name="bookingRef">The booking reference to find existing booking that needs updating.</param>
	    /// <param name="days">The new array of days to change to.</param>
	    /// <param name="roomNum">The room number to update the booking.</param>
	    /// <returns>A value indicating if the update booking was successful. True means successful, false otherwise.</returns>
	    public boolean updateBooking(String bookingRef, Integer[] days, int roomNum) throws NoSuchBookingException
	    {
	    	
	    	try 
	    	{								
				if(!bookingRefRoomNumMap.containsKey(bookingRef))
		    	{
		    		// throw NoSuchBookingException if the reference is not found.
		        	throw new NoSuchBookingException(bookingRef);
		    	}
		    	
		    	// Get the room object that needs updating.
		    	Room roomToUpdate = roomMap.get(roomNum);
		    	
		    	// check if we can update the rooms for given reference and room number.
		    	if(canUpdate(bookingRef, days, roomNum))
		    	{
		    		lock.acquire();
		    		
		    		// updating the existing booking.
		            roomToUpdate.book(bookingRef, days);
		            
		            lock.release();
		            return true;
		    	}
			} 
	    	catch (InterruptedException e) 
	    	{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	
	    	
	    	lock.release();    	
	    	return false;
	    }
	    
	    /// <summary> Cancels the existing booking, removes it from the system. </summary>
	    /// <param name="bookingRef">The booking reference to remove.</param>
	    public void cancelBooking(String bookingRef) throws NoSuchBookingException 
	    {
	    	try 
	    	{								
				// Check if given booking reference is a valid one.
		        if (!bookingRefRoomNumMap.containsKey(bookingRef))
		        {
		            throw new NoSuchBookingException(bookingRef);
		        }
		        
		        // since we are not given the room number, we have to get all those room numbers for which this booking reference was made.
		        List<Integer> roomNums = new ArrayList<Integer>();
		        roomNums = bookingRefRoomNumMap.get(bookingRef);
		        
		        lock.acquire();
		        
		        // loop through each room and cancel the booking.
		        for(int nextRoomNum : roomNums)
		        {
		        	Room bookingToCancel = roomMap.get(nextRoomNum);
		        	bookingToCancel.cancelBooking(bookingRef);
		        }
		        
		        // once all the rooms booking is cancelled, then remove the reference from bookingRefRoomNumMap hashtable.
		        bookingRefRoomNumMap.remove(bookingRef);
		        
		        lock.release();
		        
		        System.out.println("\n *** Booking is cancelled for ref: " + bookingRef + " ***\n");		        		        
			} 
	    	catch (InterruptedException e) 
	    	{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}	    		    		    	
	    }
	    
	    /// <summary> Checks if the specified rooms are booked for the given days. </summary>
	    /// <param name="days">The given days for booking.</param>
	    /// <param name="roomNums">The room numbers to check for booking on particular days.</param>
	    public boolean roomsBooked(Integer[] days, int[] roomNums)
	    {
	    	for(int nextRoomNum : roomNums)
	    	{
	    		if(this.roomBooked(days, nextRoomNum))
	    		{
	    			return true;
	    		}
	    	}
	    	
	    	return false;
	    }
	    
	    public boolean bookRooms(String bookingRef, Integer[] days, int[] roomNums) throws NoSuchBookingException
	    {
	    	try 
	    	{				
				if(bookingRefRoomNumMap.containsKey(bookingRef)) 
		    	{
		    		// throw exception because bookingRef must be a new booking reference.
		            // the client must update if existing booking reference is used.
		    		throw new NoSuchBookingException(bookingRef);
		    	}
		    	
		    	// Added list to fix HashMap's put method where we are adding
		    	// String,List<Integer> (key,value) pair
		    	List<Integer> roomNumsList = new ArrayList<Integer>();
		    	
		    	if(!roomsBooked(days, roomNums))
		    	{
		    		lock.acquire();
		    		
		    		 // this means we can book given days in given room numbers.
		    		for(int nextRoomNum : roomNums)
		    		{
		    			roomNumsList.add(nextRoomNum);
		    			Room roomToBook = roomMap.get(nextRoomNum);
		    			roomToBook.book(bookingRef, days);
		    		}
		    		
		    		bookingRefRoomNumMap.put(bookingRef, roomNumsList);
		    		
		    		lock.release();
		    		return true;
		    	}
			} 
	    	catch (InterruptedException e) 
	    	{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	
	    	
	    	lock.release();
	    	return false;
	    }
	    
	    /// <summary> Updates the existing booking.  </summary>
        /// <param name="bookingRef">The booking reference to find existing booking that needs updating.</param>
        /// <param name="days">The new array of days to change to.</param>
        /// <param name="roomNum">The room number to update the booking.</param>
        /// <returns>A value indicating if the update booking was successful. True means successful, false otherwise.</returns>
	    public boolean updateBooking(String bookingRef, Integer[] days, int[] roomNums) throws NoSuchBookingException
	    {
	    	try 
	    	{								
				if(!bookingRefRoomNumMap.containsKey(bookingRef))
		    	{
		    		throw new NoSuchBookingException(bookingRef);
		    	}
		    	
		    	// loop through the list of rooms and check if update is possible.
		    	for(int nextRoomNum : roomNums)
		    	{
		    		if(!canUpdate(bookingRef, days, nextRoomNum))
		    		{		    			
		    			return false;
		    		}    		
		    	}
		    	
		    	lock.acquire();
		    	
		    	// Reaching to this part of the code means we can update, so will update all the bookings
		    	for(int nextRoomNumber : roomNums)
		    	{
		    		Room roomToUpdate = roomMap.get(nextRoomNumber);
		    		roomToUpdate.book(bookingRef, days);
		    	}	    	
			} 
	    	catch (InterruptedException e) 
	    	{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	
	    	lock.release();
	    	return true;
	    }
	    
	    /// <summary> Determines if existing booking can be update with given days. </summary>
        /// <param name="bookingRef">Existing booking reference.</param>
        /// <param name="days">New days to update.</param>
        /// <param name="roomNum">The room number.</param>
        /// <returns>A value indicating if its okay to update. True means it can be update, false otherwise.</returns>
	    private boolean canUpdate(String bookingRef, Integer[] days, int roomNum)
	    {
	    	Room roomToUpdate = roomMap.get(roomNum);
	    	
	        // Get all the booked days for roomNum room skipping this bookingRef 
	        // because we do not want to check the booking reference that we want to update.
	    	// HashSet<Integer> bookedWithOtherBookingRef = new HashSet<Integer>();
	    	
	    	Set<Integer> bookedWithOtherBookingRef = roomToUpdate.getAllBookedDays(bookingRef);
	    	
	    	
	    	if(bookedWithOtherBookingRef.size() == 0)
	    	{
	    		// if there is nothing booked for this room, then we can update given booking reference.
	    		return true;
	    	}
	    	
	    	// bookedWithOtherBookingRef holds all the booked days for roomNum room number except bookingRef booking reference.
	        // loop through each day from the input and check if there is any match found in the already booked days.
	    	for(int nextDay : days)
	    	{
	    		if(bookedWithOtherBookingRef.contains(nextDay))
	    		{
	    			// we have found the match with other booking reference than the one we want to update
	                // therefore, we cannot update this booking reference.
	                return false;
	    		}	    		
	    	}	    	
	    	// return true when there is no conflict between all the days that are booked for the room (roomNum)
	        // and given array of days.
	        return true;	    	
	    }	    
}