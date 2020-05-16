namespace MurtazaAssignement
{
    using System;
    using System.Collections.Generic;

    /// <summary> Represents a room object in a hotel. It keeps track of the bookings (bookingRef) and the days
    /// that booking is valid for.</summary>
    public class Room
    {
        /// <summary> Maps the bookingRef with the days booked for this room. </summary>
        private readonly IDictionary<string, HashSet<int>> bookingRefDaysMapper;
        
        public Room(int number)
        {
            this.Number = number;
            bookingRefDaysMapper = new Dictionary<string, HashSet<int>>();
        }

        /// <summary> Gets or sets the room number. </summary>
        public int Number { get; set; }


        /// <summary> Books this room for given array of days with booking reference. </summary>
        /// <param name="bookingRef">The reference booking.</param>
        /// <param name="days">Array of days to book this room.</param>
        public void Book(string bookingRef, int[] days)
        {
            // if the booking reference already exists then we should update it, 
            // if its a new one then we should add.
            // if is safe to assume that there is no conflict for the days that are being passed in 
            // because the Hotel class already checks them.
            if (!bookingRefDaysMapper.ContainsKey(bookingRef))
            {
                // add new booking.
                bookingRefDaysMapper.Add(bookingRef, new HashSet<int>(days));
            }
            else
            {
                // update booking reference.
                bookingRefDaysMapper[bookingRef] = new HashSet<int>(days);
            }
        }

        /// <summary> Gets all the days that are booked for this room regardless of the booking reference. </summary>
        /// <returns>The array containing the days that are booked for this room.</returns>
        public HashSet<int> GetAllBookedDays()
        {
            HashSet<int> daysBooked = new HashSet<int>();
            
            // loop through all the bookingRefs that contain array of days of booking.
            foreach (HashSet<int> days in bookingRefDaysMapper.Values)
            {
                // add each day of booking into the array to return.
                foreach (int day in days)
                {
                    daysBooked.Add(day);
                }
            }

            return daysBooked;
        }

        /// <summary> Gets all the days that are booked for this room except for the given booking reference. </summary>
        /// <param name="skipBookingRef">The reference to skip.</param>
        /// <returns>The array containing the days that are booked for this room except the given bookingRef.</returns>
        public HashSet<int> GetAllBookedDays(string skipBookingRef)
        {
            HashSet<int> daysBooked = new HashSet<int>();
            
            foreach (string key in bookingRefDaysMapper.Keys)
            {
                // skip the given booking reference and only return those booked for other booking references.
                if (key != skipBookingRef)
                {
                    foreach (int day in bookingRefDaysMapper[key])
                    {
                        daysBooked.Add(day);
                    }
                }
            }

            return daysBooked;
        }

        /// <summary> Removes the booking days for given reference. </summary>
        /// <param name="bookingRef">The reference to remove.</param>
        public void CancelBooking(string bookingRef)
        {
            // if we already have a reference then remove it.
            if (bookingRefDaysMapper.ContainsKey(bookingRef))
            {
                bookingRefDaysMapper.Remove(bookingRef);
            }
            else
            {
                // throw NoSuchBookingException if the reference is invalid or already removed / deleted.
                throw new NoSuchBookingException();
            }
        }
    }

    public class NoSuchBookingException : Exception { }
}