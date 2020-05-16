namespace MurtazaAssignement
{
    using System;
    using System.Collections.Generic;

    /// <summary> Represents a room object in a hotel. </summary>
    public class Room
    {
        private readonly IDictionary<string, int[]> bookingRefDaysMapper;
        
        public Room(int number)
        {
            this.Number = number;
            bookingRefDaysMapper = new Dictionary<string, int[]>();
        }

        /// <summary> Gets or sets the room number. </summary>
        public int Number { get; set; }


        public void Book(string bookingRef, int[] days)
        {
            if (!bookingRefDaysMapper.ContainsKey(bookingRef))
            {
                // add new booking.
                bookingRefDaysMapper.Add(bookingRef, days);
            }
            else
            {
                // update booking reference.
                bookingRefDaysMapper[bookingRef] = days;
            }
        }

        public int[] GetAllBookedDays()
        {
            List<int> daysBooked = new List<int>();
            
            foreach (int[] days in bookingRefDaysMapper.Values)
            {
                foreach (int day in days)
                {
                    daysBooked.Add(day);
                }
            }

            return daysBooked.ToArray();
        }

        public int[] GetAllBookedDays(string skipBookingRef)
        {
            List<int> daysBooked = new List<int>();
            foreach (string key in bookingRefDaysMapper.Keys)
            {
                if (key != skipBookingRef)
                {
                    foreach (int day in bookingRefDaysMapper[key])
                    {
                        daysBooked.Add(day);
                    }
                }
            }

            return daysBooked.ToArray();
        }

        public void CancelBooking(string bookingRef)
        {
            if (bookingRefDaysMapper.ContainsKey(bookingRef))
            {
                bookingRefDaysMapper.Remove(bookingRef);
            }
            else
            {
                throw new NoSuchBookingException();
            }
        }
    }

    public class NoSuchBookingException : Exception { }
}