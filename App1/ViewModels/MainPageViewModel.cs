namespace App1.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using App1.AutoChecker;
    using App1.Models;
    using App1.Services;
    using LiveChartsCore;
    using LiveChartsCore.Kernel.Sketches;
    using LiveChartsCore.SkiaSharpView;

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

        public MainPageViewModel(
            IReviewService reviewsService,
            IUserService userService,
            IUpgradeRequestsService upgradeRequestsService,
            ICheckersService checkersService,
            IAutoCheck autoCheck)
        {
            this.reviewsService = reviewsService ?? throw new ArgumentNullException(nameof(reviewsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.requestsService = upgradeRequestsService ?? throw new ArgumentNullException(nameof(upgradeRequestsService));
            this.checkersService = checkersService ?? throw new ArgumentNullException(nameof(checkersService));
            this.autoCheck = autoCheck ?? throw new ArgumentNullException(nameof(autoCheck));

            this.InitializeCommands();

            this.LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            get => this.flaggedReviews;
            set
            {
                this.flaggedReviews = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<User> AppealsUsers
        {
            get => this.appealsUsers;
            set
            {
                this.appealsUsers = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<UpgradeRequest> UpgradeRequests
        {
            get => this.upgradeRequests;
            set
            {
                this.upgradeRequests = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> OffensiveWords
        {
            get => this.offensiveWords;
            set
            {
                this.offensiveWords = value;
                this.OnPropertyChanged();
            }
        }

        public ISeries[] PieChartSeries
        {
            get => this.pieChartSeries;
            set
            {
                this.pieChartSeries = value;
                this.OnPropertyChanged();
            }
        }

        public ISeries[] BarChartSeries
        {
            get => this.barChartSeries;
            set
            {
                this.barChartSeries = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartXAxes
        {
            get => this.barChartXAxes;
            set
            {
                this.barChartXAxes = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartYAxes
        {
            get => this.barChartYAxes;
            set
            {
                this.barChartYAxes = value;
                this.OnPropertyChanged();
            }
        }

        public User SelectedAppealUser
        {
            get => this.selectedAppealUser;
            set
            {
                this.selectedAppealUser = value;
                if (value != null)
                {
                    this.LoadUserAppealDetails(value);
                }

                this.OnPropertyChanged();
            }
        }

        public UpgradeRequest SelectedUpgradeRequest
        {
            get => this.selectedUpgradeRequest;
            set
            {
                this.selectedUpgradeRequest = value;
                if (value != null)
                {
                    this.LoadUpgradeRequestDetails(value);
                }

                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsFormatted
        {
            get => this.userReviewsFormatted;
            set
            {
                this.userReviewsFormatted = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsWithFlags
        {
            get => this.userReviewsWithFlags;
            set
            {
                this.userReviewsWithFlags = value;
                this.OnPropertyChanged();
            }
        }

        public string UserStatusDisplay
        {
            get => this.userStatusDisplay;
            set
            {
                this.userStatusDisplay = value;
                this.OnPropertyChanged();
            }
        }

        public string UserUpgradeInfo
        {
            get => this.userUpgradeInfo;
            set
            {
                this.userUpgradeInfo = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsAppealUserBanned
        {
            get => this.isAppealUserBanned;
            set
            {
                this.isAppealUserBanned = value;
                this.UserStatusDisplay = this.GetUserStatusDisplay(this.SelectedAppealUser, value);
                this.OnPropertyChanged();
            }
        }

        public bool IsWordListVisible
        {
            get => this.isWordListVisible;
            set
            {
                this.isWordListVisible = value;
                this.OnPropertyChanged();
            }
        }

        public void LoadAllData()
        {
            this.LoadFlaggedReviews();
            this.LoadAppeals();
            this.LoadRoleRequests();
            this.LoadStatistics();
            this.LoadOffensiveWords();
        }

        public void LoadFlaggedReviews()
        {
            this.FlaggedReviews = new ObservableCollection<Review>(this.reviewsService.GetFlaggedReviews());
        }

        public void LoadAppeals()
        {
            this.AppealsUsers = new ObservableCollection<User>(this.userService.GetBannedUsersWhoHaveSubmittedAppeals());
        }

        public void LoadRoleRequests()
        {
            this.UpgradeRequests = new ObservableCollection<UpgradeRequest>(this.requestsService.RetrieveAllUpgradeRequests());
        }

        public void LoadOffensiveWords()
        {
            this.OffensiveWords = new ObservableCollection<string>(this.checkersService.GetOffensiveWordsList());
        }

        public void LoadStatistics()
        {
            this.LoadPieChart();
            this.LoadBarChart();
        }

        public void FilterReviews(string filter)
        {
            this.FlaggedReviews = new ObservableCollection<Review>(
                this.reviewsService.FilterReviewsByContent(filter));
        }

        public void FilterAppeals(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                this.LoadAppeals();
                return;
            }

            filter = filter.ToLower();
            this.AppealsUsers = new ObservableCollection<User>(
                this.userService.GetBannedUsersWhoHaveSubmittedAppeals()
                    .Where(user =>
                        user.EmailAddress.ToLower().Contains(filter) ||
                        user.FullName.ToLower().Contains(filter) ||
                        user.UserId.ToString().Contains(filter))
                    .ToList());
        }

        public void ResetReviewFlags(int reviewId)
        {
            this.reviewsService.ResetReviewFlags(reviewId);
            this.LoadFlaggedReviews();
            this.LoadStatistics();
        }

        public void HideReview(int userId, int reviewId)
        {
            this.reviewsService.HideReview(userId);
            this.reviewsService.ResetReviewFlags(reviewId);
            this.LoadFlaggedReviews();
            this.LoadStatistics();
        }

        public void RunAICheck(Review review)
        {
            this.checkersService.RunAICheckForOneReview(review);
            this.LoadFlaggedReviews();
        }

        public List<string> RunAutoCheck()
        {
            List<Review> reviews = this.reviewsService.GetFlaggedReviews();
            List<string> messages = this.checkersService.RunAutoCheck(reviews);
            this.LoadFlaggedReviews();
            this.LoadStatistics();
            return messages;
        }

        public void AddOffensiveWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                this.checkersService.AddOffensiveWord(word);
                this.LoadOffensiveWords();
            }
        }

        public void DeleteOffensiveWord(string word)
        {
            this.checkersService.DeleteOffensiveWord(word);
            this.LoadOffensiveWords();
        }

        public void HandleUpgradeRequest(bool approve, int requestId)
        {
            this.requestsService.ProcessUpgradeRequest(approve, requestId);
            this.LoadRoleRequests();
            this.LoadStatistics();
        }

        public void CloseAppealCase(User user)
        {
            user.HasSubmittedAppeal = false;
            this.LoadAppeals();
        }

        public List<Review> GetUserReviews(int userId)
        {
            return this.reviewsService.GetReviewsByUser(userId);
        }

        public User GetUserById(int userId)
        {
            return this.userService.GetUserById(userId);
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            return this.userService.GetHighestRoleTypeForUser(userId);
        }

        public string GetRoleNameBasedOnID(RoleType roleType)
        {
            return this.requestsService.GetRoleNameBasedOnIdentifier(roleType);
        }

        public void LoadUserAppealDetails(User user)
        {
            this.SelectedAppealUser = user;
            this.IsAppealUserBanned = true;
            this.UserStatusDisplay = this.GetUserStatusDisplay(user, true);

            List<Review> reviews = this.GetUserReviews(user.UserId);
            this.UserReviewsFormatted = new ObservableCollection<string>(
                reviews.Select(r => this.FormatReviewContent(r)).ToList());
        }

        public void KeepBanForUser(User user)
        {
            if (user == null)
            {
                return;
            }

            this.UpdateUserRole(user, RoleType.Banned);
            this.IsAppealUserBanned = true;
            this.UserStatusDisplay = this.GetUserStatusDisplay(user, true);
            this.LoadStatistics();
        }

        public void AcceptAppealForUser(User user)
        {
            if (user == null)
            {
                return;
            }

            this.UpdateUserRole(user, RoleType.User);
            this.IsAppealUserBanned = false;
            this.UserStatusDisplay = this.GetUserStatusDisplay(user, false);
            this.LoadStatistics();
        }

        public void LoadUpgradeRequestDetails(UpgradeRequest request)
        {
            this.SelectedUpgradeRequest = request;

            int userId = request.RequestingUserIdentifier;
            User selectedUser = this.GetUserById(userId);
            RoleType currentRoleID = this.GetHighestRoleTypeForUser(selectedUser.UserId);
            string currentRoleName = this.GetRoleNameBasedOnID(currentRoleID);
            string requiredRoleName = this.GetRoleNameBasedOnID(currentRoleID + 1);

            this.UserUpgradeInfo = this.FormatUserUpgradeInfo(selectedUser, currentRoleName, requiredRoleName);

            List<Review> reviews = this.GetUserReviews(selectedUser.UserId);
            this.UserReviewsWithFlags = new ObservableCollection<string>(
                reviews.Select(r => this.FormatReviewWithFlags(r)).ToList());
        }

        public string GetUserStatusDisplay(User user, bool isBanned)
        {
            if (user == null)
            {
                return string.Empty;
            }

            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\nStatus: {(isBanned ? "Banned" : "Active")}";
        }

        public string FormatUserUpgradeInfo(User user, string currentRoleName, string requiredRoleName)
        {
            if (user == null)
            {
                return string.Empty;
            }

            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\n{currentRoleName} -> {requiredRoleName}";
        }

        public string FormatReviewContent(Review review)
        {
            if (review == null)
            {
                return string.Empty;
            }

            return $"{review.Content}";
        }

        public string FormatReviewWithFlags(Review review)
        {
            if (review == null)
            {
                return string.Empty;
            }

            return $"{review.Content}\nFlags: {review.NumberOfFlags}";
        }

        public void ShowWordListPopup()
        {
            this.IsWordListVisible = true;
        }

        public void HideWordListPopup()
        {
            this.IsWordListVisible = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadBarChart()
        {
            int rejectedCount = this.reviewsService.GetHiddenReviews().Count;
            int pendingCount = this.reviewsService.GetFlaggedReviews().Count;
            int totalCount = this.reviewsService.GetAllReviews().Count;

            this.BarChartSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { rejectedCount, pendingCount, totalCount },
                },
            };

            this.BarChartXAxes = new[]
            {
                new Axis { Labels = new List<string> { "rejected", "pending", "total" } },
            };

            this.BarChartYAxes = new[]
            {
                new Axis { Name = "Total", MinLimit = 0 },
            };
        }

        private void InitializeCommands()
        {
            this.KeepBanCommand = new RelayCommand(() => this.KeepBanForUser(this.SelectedAppealUser));
            this.AcceptAppealCommand = new RelayCommand(() => this.AcceptAppealForUser(this.SelectedAppealUser));
            this.CloseAppealCaseCommand = new RelayCommand(() => this.CloseAppealCase(this.SelectedAppealUser));

            this.HandleUpgradeRequestCommand = new RelayCommand<Tuple<bool, int>>(param =>
                this.HandleUpgradeRequest(param.Item1, param.Item2));

            this.ResetReviewFlagsCommand = new RelayCommand<int>(reviewId =>
                this.ResetReviewFlags(reviewId));

            this.HideReviewCommand = new RelayCommand<Tuple<int, int>>(param =>
                this.HideReview(param.Item1, param.Item2));

            this.RunAICheckCommand = new RelayCommand<Review>(review =>
                this.RunAICheck(review));

            this.RunAutoCheckCommand = new RelayCommand(() => this.RunAutoCheck());

            this.AddOffensiveWordCommand = new RelayCommand<string>(word =>
                this.AddOffensiveWord(word));

            this.DeleteOffensiveWordCommand = new RelayCommand<string>(word =>
                this.DeleteOffensiveWord(word));

            this.ShowWordListPopupCommand = new RelayCommand(() => this.ShowWordListPopup());
            this.HideWordListPopupCommand = new RelayCommand(() => this.HideWordListPopup());
        }

        private void LoadPieChart()
        {
            int bannedCount = 0, usersCount = 0, adminsCount = 0, managerCount = 0;

            List<User> users = this.userService.GetAllUsers();
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

            this.PieChartSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { bannedCount }, Name = "Banned" },
                new PieSeries<double> { Values = new double[] { usersCount }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { adminsCount }, Name = "Admins" },
                new PieSeries<double> { Values = new double[] { managerCount }, Name = "Managers" },
            };
        }

        private void UpdateUserRole(User user, RoleType roleType)
        {
            this.userService.UpdateUserRole(user.UserId, roleType);
        }
    }
}