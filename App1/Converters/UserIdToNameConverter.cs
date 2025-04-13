using Microsoft.UI.Xaml.Data;
using App1.Services;
using System;

namespace App1.Converters
{
    public class UserIdToNameConverter : IValueConverter
    {
        private static IUserService _userService;

        public static void Initialize(IUserService userService)
        {
            _userService = userService;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int userId && _userService != null)
            {
                try
                {
                    var user = _userService.GetUserById(userId);
                    return user?.FullName ?? $"User {userId}";
                }
                catch
                {
                    return $"User {userId}";
                }
            }
            return "Unknown User";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 