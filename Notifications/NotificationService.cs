using OrderNotifications.Models;

namespace OrderNotifications
{
    public class NotificationService : INotificationService
    {
        public void NotifyOrderStatus(Order order)
        {
            var customer = order.Customer;
            var country= customer.CountryCode.ToUpper();
            var message = $"Customer {customer.Name}, your order {order.Id} has been sent.";
            var channels = new[] {NotificationChannel.Email,NotificationChannel.SMS,NotificationChannel.WhatsApp};

            foreach (var channel in channels)
            {
                var enabledChannel = GetNotificationChannel(customer, channel);
                if (enabledChannel != null)
                {
                    switch (enabledChannel)
                    {
                        case NotificationChannel.Email:
                            if (country != "AK") { Console.WriteLine($"To Email {customer.ContactInfo.Email}: {message}"); };
                            break;
                        case NotificationChannel.SMS:
                            Console.WriteLine($"To SMS {customer.ContactInfo.PhoneNumber}: {message}");
                            break;
                        case NotificationChannel.WhatsApp:
                            Console.WriteLine($"To Whatsapp {customer.ContactInfo.PhoneNumber}: {message}");
                            break;
                    }
                }
            }
        }
        private NotificationChannel? GetNotificationChannel(Customer customer, NotificationChannel channel)
        {
            var country = customer.CountryCode.ToUpper();
            var preferences = customer.Preferences;
            bool confprf = preferences.TryGetValue(channel, out var pref);

            bool enable = channel switch
            {
                NotificationChannel.Email =>
                    country == "AK"
                        ? confprf && pref == NotificationChannelPreference.Enabled
                        : !confprf || pref != NotificationChannelPreference.Disabled,

                NotificationChannel.SMS =>
                    (country == "AK" || country == "MX")
                        ? confprf && pref == NotificationChannelPreference.Enabled
                        : !confprf || pref != NotificationChannelPreference.Disabled,

                NotificationChannel.WhatsApp =>
                    country == "AK"
                        ? confprf && pref == NotificationChannelPreference.Enabled
                        : country == "MX"
                            ? !confprf || pref != NotificationChannelPreference.Disabled
                            : confprf && pref == NotificationChannelPreference.Enabled,

                _ => false
            };

            return enable ? channel : null;
        }

    }
}
