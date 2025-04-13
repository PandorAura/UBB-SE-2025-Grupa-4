using App1.Models;
using App1.Services;
using App1.AutoChecker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Kernel.Sketches;
using System.Windows.Input;

namespace App1.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IReviewService reviewsService;
        private readonly IUserService userService;
        private readonly ICheckersService checkersService;
        private readonly IUpgradeRequestsService requestsService;
        private readonly IAutoCheck autoCheck;

        private ObservableCollection<Review> flaggedReviews;
        private ObservableCollection<User> appealsUsers;
        private ObservableCollection<UpgradeRequest> upgradeRequests;
        private ObservableCollection<string> offensiveWords;
        private ISeries[] pieChartSeries;
        private ISeries[] barChartSeries;
        private IEnumerable<ICartesianAxis> barChartXAxes;
        private IEnumerable<ICartesianAxis> barChartYAxes;
        private User selectedAppealUser;
        private UpgradeRequest selectedUpgradeRequest;
        private ObservableCollection<string> userReviewsFormatted;
        private ObservableCollection<string> userReviewsWithFlags;
        private string userStatusDisplay;
        private string userUpgradeInfo;
        private bool isAppealUserBanned = true;
        private bool isWordListVisible = false;

        public ICommand KeepBanCommand { get; private set; }

        public ICommand AcceptAppealCommand { get; private set; }

        public ICommand CloseAppealCaseCommand { get; private set; }

        public ICommand HandleUpgradeRequestCommand { get; private set; }

        public ICommand ResetReviewFlagsCommand { get; private set; }

        public ICommand HideReviewCommand { get; private set; }

        public ICommand RunAICheckCommand { get; private set; }

        public ICommand RunAutoCheckCommand { get; private set; }

        public ICommand AddOffensiveWordCommand { get; private set; }

        public ICommand DeleteOffensiveWordCommand { get; private set; }

        public ICommand ShowWordListPopupCommand { get; private set; }

        public ICommand HideWordListPopupCommand { get; private set; }

        public ObservableCollection<Review> FlaggedReviews
        {
            get => flaggedReviews;
            set
            {
                flaggedReviews = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> AppealsUsers
        {
            get => appealsUsers;
            set
            {
                appealsUsers = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UpgradeRequest> UpgradeRequests
        {
            get => upgradeRequests;
            set
            {
                upgradeRequests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> OffensiveWords
        {
            get => offensiveWords;
            set
            {
                offensiveWords = value;
                OnPropertyChanged();
            }
        }

        public ISeries[] PieChartSeries
        {
            get => pieChartSeries;
            set
            {
                pieChartSeries = value;
                OnPropertyChanged();
            }
        }

        public ISeries[] BarChartSeries
        {
            get => barChartSeries;
            set
            {
                barChartSeries = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartXAxes
        {
            get => barChartXAxes;
            set
            {
                barChartXAxes = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartYAxes
        {
            get => barChartYAxes;
            set
            {
                barChartYAxes = value;
                OnPropertyChanged();
            }
        }

        public User SelectedAppealUser
        {
            get => selectedAppealUser;
            set
            {
                selectedAppealUser = value;
                if (value != null)
                {
                    LoadUserAppealDetails(value);
                }

                OnPropertyChanged();
            }
        }

        public UpgradeRequest SelectedUpgradeRequest
        {
            get => selectedUpgradeRequest;
            set
            {
                selectedUpgradeRequest = value;
                if (value != null)
                {
                    LoadUpgradeRequestDetails(value);
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsFormatted
        {
            get => userReviewsFormatted;
            set
            {
                userReviewsFormatted = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsWithFlags
        {
            get => userReviewsWithFlags;
            set
            {
                userReviewsWithFlags = value;
                OnPropertyChanged();
            }
        }

        public string UserStatusDisplay
        {
            get => userStatusDisplay;
            set
            {
                userStatusDisplay = value;
                OnPropertyChanged();
            }
        }

        public string UserUpgradeInfo
        {
            get => userUpgradeInfo;
            set
            {
                userUpgradeInfo = value;
                OnPropertyChanged();
            }
        }

        public bool IsAppealUserBanned
        {
            get => isAppealUserBanned;
            set
            {
                isAppealUserBanned = value;
                UserStatusDisplay = GetUserStatusDisplay(SelectedAppealUser, value);
                OnPropertyChanged();
            }
        }

        public bool IsWordListVisible
        {
            get => isWordListVisible;
            set
            {
                isWordListVisible = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel(
            IReviewService reviewsService,
            IUserService userService,
            IUpgradeRequestsService upgradeRequestsService,
            ICheckersService checkersService,
            IAutoCheck autoCheck)
        {
            _reviewsService = reviewsService ?? throw new ArgumentNullException(nameof(reviewsService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _requestsService = upgradeRequestsService ?? throw new ArgumentNullException(nameof(upgradeRequestsService));
            _checkersService = checkersService ?? throw new ArgumentNullException(nameof(checkersService));
            _autoCheck = autoCheck ?? throw new ArgumentNullException(nameof(autoCheck));

            InitializeCommands();

            LoadAllData();
        }

        private void InitializeCommands()
        {
            KeepBanCommand = new RelayCommand(() => KeepBanForUser(SelectedAppealUser));
            AcceptAppealCommand = new RelayCommand(() => AcceptAppealForUser(SelectedAppealUser));
            CloseAppealCaseCommand = new RelayCommand(() => CloseAppealCase(SelectedAppealUser));
            
            HandleUpgradeRequestCommand = new RelayCommand<Tuple<bool, int>>(param => 
                HandleUpgradeRequest(param.Item1, param.Item2));
            
            ResetReviewFlagsCommand = new RelayCommand<int>(reviewId => 
                ResetReviewFlags(reviewId));
            
            HideReviewCommand = new RelayCommand<Tuple<int, int>>(param => 
                HideReview(param.Item1, param.Item2));
            
            RunAICheckCommand = new RelayCommand<Review>(review => 
                RunAICheck(review));
            
            RunAutoCheckCommand = new RelayCommand(() => RunAutoCheck());
            
            AddOffensiveWordCommand = new RelayCommand<string>(word => 
                AddOffensiveWord(word));
            
            DeleteOffensiveWordCommand = new RelayCommand<string>(word => 
                DeleteOffensiveWord(word));
            
            ShowWordListPopupCommand = new RelayCommand(() => ShowWordListPopup());
            HideWordListPopupCommand = new RelayCommand(() => HideWordListPopup());
        }

        public void LoadAllData()
        {
            LoadFlaggedReviews();
            LoadAppeals();
            LoadRoleRequests();
            LoadStatistics();
            LoadOffensiveWords();
        }

        public void LoadFlaggedReviews()
        {
            FlaggedReviews = new ObservableCollection<Review>(reviewsService.GetFlaggedReviews());
        }

        public void LoadAppeals()
        {
            AppealsUsers = new ObservableCollection<User>(userService.GetBannedUsersWhoHaveSubmittedAppeals());
        }

        public void LoadRoleRequests()
        {
            UpgradeRequests = new ObservableCollection<UpgradeRequest>(requestsService.RetrieveAllUpgradeRequests());
        }

        public void LoadOffensiveWords()
        {
            OffensiveWords = new ObservableCollection<string>(checkersService.GetOffensiveWordsList());
        }

        public void LoadStatistics()
        {
            LoadPieChart();
            LoadBarChart();
        }

        public void FilterReviews(string filter)
        {
            FlaggedReviews = new ObservableCollection<Review>(
                reviewsService.FilterReviewsByContent(filter));
        }

        public void FilterAppeals(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                LoadAppeals();
                return;
            }

            filter = filter.ToLower();
            AppealsUsers = new ObservableCollection<User>(
                userService.GetBannedUsersWhoHaveSubmittedAppeals()
                    .Where(user =>
                        user.EmailAddress.ToLower().Contains(filter) ||
                        user.FullName.ToLower().Contains(filter) ||
                        user.UserId.ToString().Contains(filter))
                    .ToList());
        }

        public void ResetReviewFlags(int reviewId)
        {
            reviewsService.ResetReviewFlags(reviewId);
            LoadFlaggedReviews();
            LoadStatistics();
        }

        public void HideReview(int userId, int reviewId)
        {
            reviewsService.HideReview(userId);
            reviewsService.ResetReviewFlags(reviewId);
            LoadFlaggedReviews();
            LoadStatistics();
        }

        public void RunAICheck(Review review)
        {
            _checkersService.RunAICheckForOneReview(review);
            LoadFlaggedReviews();
        }

        public List<string> RunAutoCheck()
        {
            List<Review> reviews = reviewsService.GetFlaggedReviews();
            List<string> messages = checkersService.RunAutoCheck(reviews);
            LoadFlaggedReviews();
            LoadStatistics();
            return messages;
        }

        public void AddOffensiveWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                checkersService.AddOffensiveWord(word);
                LoadOffensiveWords();
            }
        }

        public void DeleteOffensiveWord(string word)
        {
            checkersService.DeleteOffensiveWord(word);
            LoadOffensiveWords();
        }

        public void HandleUpgradeRequest(bool approve, int requestId)
        {
            requestsService.ProcessUpgradeRequest(approve, requestId);
            LoadRoleRequests();
            LoadStatistics();
        }

        public void CloseAppealCase(User user)
        {
            user.HasSubmittedAppeal = false;
            LoadAppeals();
        }

        public List<Review> GetUserReviews(int userId)
        {
            return reviewsService.GetReviewsByUser(userId);
        }

        public User GetUserById(int userId)
        {
            return userService.GetUserById(userId);
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            return userService.GetHighestRoleTypeForUser(userId);
        }

        public string GetRoleNameBasedOnID(RoleType roleType)
        {
            return _requestsService.GetRoleNameBasedOnIdentifier(roleType);
        }

        private void LoadPieChart()
        {
            int bannedCount = 0, usersCount = 0, adminsCount = 0, managerCount = 0;

            List<User> users = _userService.GetAllUsers();
            foreach (User user in users)
            {
                
                int count = user.AssignedRoles.Count;
                switch (count)
                {
                    case 0: bannedCount++; break;
                    case 1: usersCount++; break;
                    case 2: adminsCount++; break;
                    case 3: managerCount++; break;
                }
            }

            PieChartSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { bannedCount }, Name = "Banned" },
                new PieSeries<double> { Values = new double[] { usersCount }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { adminsCount }, Name = "Admins" },
                new PieSeries<double> { Values = new double[] { managerCount }, Name = "Managers" }
            };
        }

        private void LoadBarChart()
        {
            int rejectedCount = _reviewsService.GetHiddenReviews().Count;
            int pendingCount = _reviewsService.GetFlaggedReviews().Count;
            int totalCount = _reviewsService.GetAllReviews().Count;

            BarChartSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { rejectedCount, pendingCount, totalCount }
                }
            };

            BarChartXAxes = new[] 
            {
                new Axis { Labels = new List<string> { "rejected", "pending", "total" } }
            };

            BarChartYAxes = new[] 
            { 
                new Axis { Name = "Total", MinLimit = 0 } 
            };
        }

        public void LoadUserAppealDetails(User user)
        {
            SelectedAppealUser = user;
            IsAppealUserBanned = true;
            UserStatusDisplay = GetUserStatusDisplay(user, true);
            
            List<Review> reviews = GetUserReviews(user.UserId);
            UserReviewsFormatted = new ObservableCollection<string>(
                reviews.Select(r => FormatReviewContent(r)).ToList());
        }

        public void KeepBanForUser(User user)
        {
            if (user == null) { return; }
            UpdateUserRole(user, RoleType.Banned);
            IsAppealUserBanned = true;
            UserStatusDisplay = GetUserStatusDisplay(user, true);
            LoadStatistics();
        }

        public void AcceptAppealForUser(User user)
        {
            if (user == null) { return; }
            UpdateUserRole(user, RoleType.User);
            IsAppealUserBanned = false;
            UserStatusDisplay = GetUserStatusDisplay(user, false);
            LoadStatistics();
        }

        public void LoadUpgradeRequestDetails(UpgradeRequest request)
        {
            SelectedUpgradeRequest = request;

            int userId = request.RequestingUserIdentifier;
            User selectedUser = GetUserById(userId);
            RoleType currentRoleID = GetHighestRoleTypeForUser(selectedUser.UserId);
            string currentRoleName = GetRoleNameBasedOnID(currentRoleID);
            string requiredRoleName = GetRoleNameBasedOnID(currentRoleID + 1);

            UserUpgradeInfo = FormatUserUpgradeInfo(selectedUser, currentRoleName, requiredRoleName);
            
            List<Review> reviews = GetUserReviews(selectedUser.UserId);
            UserReviewsWithFlags = new ObservableCollection<string>(
                reviews.Select(r => FormatReviewWithFlags(r)).ToList());
        }

        public string GetUserStatusDisplay(User user, bool isBanned)
        {
            if (user == null) { return string.Empty; }
            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\nStatus: {(isBanned ? "Banned" : "Active")}";
        }

        public string FormatUserUpgradeInfo(User user, string currentRoleName, string requiredRoleName)
        {
            if (user == null) { return string.Empty; }
            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\n{currentRoleName} -> {requiredRoleName}";
        }

        public string FormatReviewContent(Review review)
        {
            if (review == null) { return string.Empty; }
            return $"{review.Content}";
        }

        public string FormatReviewWithFlags(Review review)
        {
            if (review == null) { return string.Empty; }
            return $"{review.Content}\nFlags: {review.NumberOfFlags}";
        }

        public void ShowWordListPopup()
        {
            IsWordListVisible = true;
        }

        public void HideWordListPopup()
        {
            IsWordListVisible = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateUserRole(User user, RoleType roleType)
        {
            userService.UpdateUserRole(user.UserId, roleType);
        }

        private void InitializeCommands()
        {
            KeepBanCommand = new RelayCommand(() => KeepBanForUser(SelectedAppealUser));
            AcceptAppealCommand = new RelayCommand(() => AcceptAppealForUser(SelectedAppealUser));
            CloseAppealCaseCommand = new RelayCommand(() => CloseAppealCase(SelectedAppealUser));

            HandleUpgradeRequestCommand = new RelayCommand<Tuple<bool, int>>(param =>
                HandleUpgradeRequest(param.Item1, param.Item2));

            ResetReviewFlagsCommand = new RelayCommand<int>(reviewId =>
                ResetReviewFlags(reviewId));

            HideReviewCommand = new RelayCommand<Tuple<int, int>>(param =>
                HideReview(param.Item1, param.Item2));

            RunAICheckCommand = new RelayCommand<Review>(review =>
                RunAICheck(review));

            RunAutoCheckCommand = new RelayCommand(() => RunAutoCheck());

            AddOffensiveWordCommand = new RelayCommand<string>(word =>
                AddOffensiveWord(word));

            DeleteOffensiveWordCommand = new RelayCommand<string>(word =>
                DeleteOffensiveWord(word));

            ShowWordListPopupCommand = new RelayCommand(() => ShowWordListPopup());
            HideWordListPopupCommand = new RelayCommand(() => HideWordListPopup());
        }

        private void LoadPieChart()
        {
            int bannedCount = 0, usersCount = 0, adminsCount = 0, managerCount = 0;

            List<User> users = userService.GetAllUsers();
            foreach (var user in users)
            {
                var count = user.AssignedRoles.Count;
                switch (count)
                {
                    case 0: bannedCount++; break;
                    case 1: usersCount++; break;
                    case 2: adminsCount++; break;
                    case 3: managerCount++; break;
                }
            }

            PieChartSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { bannedCount }, Name = "Banned" },
                new PieSeries<double> { Values = new double[] { usersCount }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { adminsCount }, Name = "Admins" },
                new PieSeries<double> { Values = new double[] { managerCount }, Name = "Managers" }
            };
        }

        private void LoadBarChart()
        {
            var rejectedCount = reviewsService.GetHiddenReviews().Count;
            var pendingCount = reviewsService.GetFlaggedReviews().Count;
            var totalCount = reviewsService.GetAllReviews().Count;

            BarChartSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { rejectedCount, pendingCount, totalCount }
                }
            };

            BarChartXAxes = new[]
            {
                new Axis { Labels = new List<string> { "rejected", "pending", "total" } }
            };

            BarChartYAxes = new[]
            {
                new Axis { Name = "Total", MinLimit = 0 }
            };
        }
    }
}