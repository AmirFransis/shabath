using Shabath.DataAccess;
using Shabath.DataAccess.Models;
using Shabath.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shabath.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                if (!IsReminderForParticipates()) CreateNewKabalatShabathEvent();

                SendMailNotificationToParticipates();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void CreateNewKabalatShabathEvent()
        {
            UpdateNewRandomParticipatesFromDB();
            UpdateNewEventDateFromDB();
        }

        private static void UpdateNewEventDateFromDB()
        {
            DateTime eventDate = DateTime.Today;
            string dayOfWeek;

            using (var conn = new ShabathDBContextFactory().CreateDbContext())
            {
                dayOfWeek = conn.Rounds.First().DayOfWeek;

                var culture = new System.Globalization.CultureInfo("he-IL");
                var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
                while (!day.Equals(dayOfWeek))
                {
                    eventDate = eventDate.AddDays(1);
                    day = culture.DateTimeFormat.GetDayName(eventDate.DayOfWeek);
                }
                conn.Rounds.First().EventDate = eventDate;
                conn.SaveChanges();
            }
        }
    

        private static bool IsReminderForParticipates()
        {
            DateTime eventDate;
            bool isReminderForParticipates;

            using (var conn = new ShabathDBContextFactory().CreateDbContext())
            {
                eventDate = conn.Rounds.First().EventDate;
                isReminderForParticipates = eventDate > DateTime.Today;
            }

            return isReminderForParticipates;
        }

        private static Participates GetParticipatesFromDB()
        {
            DateTime eventDate;
            List<Members> members;

            using (var conn = new ShabathDBContextFactory().CreateDbContext())
            {
                eventDate = conn.Rounds.First().EventDate;
                members = conn.Members.Where(x => x.IsChosenForThisWeek).ToList();
            }

            return new Participates()
            {
                EventDate = eventDate,
                Members = members
            };
        }

        public enum CurrentRoundStatus
        {
            NotAvailable,
            AllMembersParticipate,
            OnlyOneLeftForParticipate,
            AtLeastTwoParticipate,
        }
        private static void UpdateNewRandomParticipatesFromDB()
        {
            Participates participates = new Participates();
            CurrentRoundStatus currentRoundStatus;
            int currentRoundNumber, nextRoundNumber;
            List<Members> optionalMembers = new List<Members>();

            using (var conn = new ShabathDBContextFactory().CreateDbContext())
            {
                currentRoundNumber = conn.Rounds.First().CurrentRoundNumber;
                optionalMembers = conn.Members.Where(x => x.IsActive && x.RoundNumber == currentRoundNumber).ToList();
                currentRoundStatus = GetCurrentRoundStatus(optionalMembers);

                switch (currentRoundStatus)
                {
                    case CurrentRoundStatus.AllMembersParticipate:
                        nextRoundNumber = ++currentRoundNumber;
                        optionalMembers = conn.Members.Where(x => x.RoundNumber == nextRoundNumber).ToList();
                        participates.Members = optionalMembers.OrderBy(x => Guid.NewGuid()).Take(2).ToList();
                        conn.Rounds.First().CurrentRoundNumber++;
                        break;

                    case CurrentRoundStatus.OnlyOneLeftForParticipate:
                        nextRoundNumber = ++currentRoundNumber;
                        participates.Members[0] = optionalMembers.First();
                        participates.Members[1] = conn.Members.Where(x => x.RoundNumber == nextRoundNumber).OrderBy(x => Guid.NewGuid()).First();
                        conn.Rounds.First().CurrentRoundNumber++;
                        break;

                    case CurrentRoundStatus.AtLeastTwoParticipate:
                        participates.Members = optionalMembers.OrderBy(x => Guid.NewGuid()).Take(2).ToList();
                        break;

                    default:
                        break;
                }

                conn.Members.ToList().ForEach(c => c.IsChosenForThisWeek = false);
                participates.Members.ForEach(x => { x.IsChosenForThisWeek = true; x.RoundNumber++; });
                
                conn.SaveChanges();
            }

        }

        private static CurrentRoundStatus GetCurrentRoundStatus(List<Members> optionalMembers)
        {
            int amountOfParticipates = optionalMembers.Count;

            if (amountOfParticipates == 0) return CurrentRoundStatus.AllMembersParticipate;
            else if (amountOfParticipates == 1) return CurrentRoundStatus.OnlyOneLeftForParticipate;
            else if (amountOfParticipates >= 2) return CurrentRoundStatus.AtLeastTwoParticipate;

            return CurrentRoundStatus.NotAvailable;
        }

        private static void SendMailNotificationToParticipates()
        {
            Participates participates;

            participates = GetParticipatesFromDB();

            MailService.SendMail(participates.Members.Select(x => x.Email).ToArray(),
                participates.EventDate);
        }
    }
}
