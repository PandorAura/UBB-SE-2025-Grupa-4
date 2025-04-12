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
    /*
     * MVVM Improvement Suggestions:
     * 
     * 1. Implement ICommand properties for all button actions to remove event handlers from code-behind
     *    - Create a RelayCommand class to implement ICommand
     *    - Example: public ICommand KeepBanCommand { get; private set; }
     *    - Initialize in constructor: KeepBanCommand = new RelayCommand<User>(user => KeepBanForUser(user));
     * 
     * 2. Create proper models with change notification instead of directly modifying properties
     *    - Create UserModel that wraps User and provides proper change notification
     *    - Implement service methods to update the model properly
     * 
     * 3. Add proper data validation with INotifyDataErrorInfo
     * 
     * 4. Move all remaining UI construction code to XAML using proper templates
     *    - Replace dynamically created flyouts with templated popups in XAML
     *    - Use proper DataTemplates for displaying user data
     * 
     * 5. Use ObservableCollection<T> for all collections that may change
     * 
     * 6. Implement a proper navigation service for managing views
     */
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IReviewService _reviewsService;
        private readonly IUserService _userService;
        private readonly ICheckersService _checkersService;
        private readonly IUpgradeRequestsService _requestsService;
        private readonly IAutoCheck _autoCheck;

        private ObservableCollection<Review> _flaggedReviews;
        private ObservableCollection<User> _appealsUsers;
        private ObservableCollection<UpgradeRequest> _upgradeRequests;
        private ObservableCollection<string> _offensiveWords;
        private ISeries[] _pieChartSeries;
        private ISeries[] _barChartSeries;
        private IEnumerable<ICartesianAxis> _barChartXAxes;
        private IEnumerable<ICartesianAxis> _barChartYAxes;
        private User _selectedAppealUser;
        private UpgradeRequest _selectedUpgradeRequest;
        private ObservableCollection<string> _userReviewsFormatted;
        private ObservableCollection<string> _userReviewsWithFlags;
        private string _userStatusDisplay;
        private string _userUpgradeInfo;
        private bool _isAppealUserBanned = true;
        private bool _isWordListVisible = false;

        #region Commands
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
        #endregion

        public ObservableCollection<Review> FlaggedReviews
        {
            get => _flaggedReviews;
            set
            {
                _flaggedReviews = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> AppealsUsers
        {
            get => _appealsUsers;
            set
            {
                _appealsUsers = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UpgradeRequest> UpgradeRequests
        {
            get => _upgradeRequests;
            set
            {
                _upgradeRequests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> OffensiveWords
        {
            get => _offensiveWords;
            set
            {
                _offensiveWords = value;
                OnPropertyChanged();
            }
        }

        public ISeries[] PieChartSeries
        {
            get => _pieChartSeries;
            set
            {
                _pieChartSeries = value;
                OnPropertyChanged();
            }
        }

        public ISeries[] BarChartSeries
        {
            get => _barChartSeries;
            set
            {
                _barChartSeries = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartXAxes
        {
            get => _barChartXAxes;
            set
            {
                _barChartXAxes = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ICartesianAxis> BarChartYAxes
        {
            get => _barChartYAxes;
            set
            {
                _barChartYAxes = value;
                OnPropertyChanged();
            }
        }

        public User SelectedAppealUser
        {
            get => _selectedAppealUser;
            set
            {
                _selectedAppealUser = value;
                if (value != null)
                {
                    LoadUserAppealDetails(value);
                }
                OnPropertyChanged();
            }
        }

        public UpgradeRequest SelectedUpgradeRequest
        {
            get => _selectedUpgradeRequest;
            set
            {
                _selectedUpgradeRequest = value;
                if (value != null)
                {
                    LoadUpgradeRequestDetails(value);
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsFormatted
        {
            get => _userReviewsFormatted;
            set
            {
                _userReviewsFormatted = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UserReviewsWithFlags
        {
            get => _userReviewsWithFlags;
            set
            {
                _userReviewsWithFlags = value;
                OnPropertyChanged();
            }
        }

        public string UserStatusDisplay
        {
            get => _userStatusDisplay;
            set
            {
                _userStatusDisplay = value;
                OnPropertyChanged();
            }
        }

        public string UserUpgradeInfo
        {
            get => _userUpgradeInfo;
            set
            {
                _userUpgradeInfo = value;
                OnPropertyChanged();
            }
        }

        public bool IsAppealUserBanned
        {
            get => _isAppealUserBanned;
            set
            {
                _isAppealUserBanned = value;
                UserStatusDisplay = GetUserStatusDisplay(SelectedAppealUser, value);
                OnPropertyChanged();
            }
        }

        public bool IsWordListVisible
        {
            get => _isWordListVisible;
            set
            {
                _isWordListVisible = value;
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

            // Initialize commands
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
            FlaggedReviews = new ObservableCollection<Review>(_reviewsService.GetFlaggedReviews());
        }

        public void LoadAppeals()
        {
            AppealsUsers = new ObservableCollection<User>(_userService.GetBannedUsersWhoHaveSubmittedAppeals());
        }

        public void LoadRoleRequests()
        {
            UpgradeRequests = new ObservableCollection<UpgradeRequest>(_requestsService.RetrieveAllUpgradeRequests());
        }

        public void LoadOffensiveWords()
        {
            OffensiveWords = new ObservableCollection<string>(_checkersService.GetOffensiveWordsList());
        }

        public void LoadStatistics()
        {
            LoadPieChart();
            LoadBarChart();
        }

        public void FilterReviews(string filter)
        {
            FlaggedReviews = new ObservableCollection<Review>(
                _reviewsService.FilterReviewsByContent(filter)
            );
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
                _userService.GetBannedUsersWhoHaveSubmittedAppeals()
                    .Where(user => 
                        user.EmailAddress.ToLower().Contains(filter) || 
                        user.FullName.ToLower().Contains(filter) ||
                        user.UserId.ToString().Contains(filter))
                    .ToList()
            );
        }

        public void ResetReviewFlags(int reviewId)
        {
            _reviewsService.ResetReviewFlags(reviewId);
            LoadFlaggedReviews();
            LoadStatistics();
        }

        public void HideReview(int userId, int reviewId)
        {
            _reviewsService.HideReview(userId);
            _reviewsService.ResetReviewFlags(reviewId);
            LoadFlaggedReviews();
            LoadStatistics();
        }

        public void RunAICheck(Review review)
        {
            _checkersService.RunAICheck(review);
            LoadFlaggedReviews();
        }

        public List<string> RunAutoCheck()
        {
            List<Review> reviews = _reviewsService.GetFlaggedReviews();
            List<string> messages = _checkersService.RunAutoCheck(reviews);
            LoadFlaggedReviews();
            LoadStatistics();
            return messages;
        }

        public void AddOffensiveWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                _checkersService.AddOffensiveWord(word);
                LoadOffensiveWords();
            }
        }

        public void DeleteOffensiveWord(string word)
        {
            _checkersService.DeleteOffensiveWord(word);
            LoadOffensiveWords();
        }

        public void HandleUpgradeRequest(bool approve, int requestId)
        {
            _requestsService.ProcessUpgradeRequest(approve, requestId);
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
            return _reviewsService.GetReviewsByUser(userId);
        }

        public User GetUserById(int userId)
        {
            return _userService.GetUserById(userId);
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            return _userService.GetHighestRoleTypeForUser(userId);
        }

        public string GetRoleNameBasedOnID(RoleType roleType)
        {
            return _requestsService.GetRoleNameBasedOnIdentifier(roleType);
        }

        private void LoadPieChart()
        {
            int bannedCount = 0, usersCount = 0, adminsCount = 0, managerCount = 0;

            List<User> users = _userService.GetAllUsers();
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
            var rejectedCount = _reviewsService.GetHiddenReviews().Count;
            var pendingCount = _reviewsService.GetFlaggedReviews().Count;
            var totalCount = _reviewsService.GetAllReviews().Count;

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
            
            var reviews = GetUserReviews(user.UserId);
            UserReviewsFormatted = new ObservableCollection<string>(
                reviews.Select(r => FormatReviewContent(r)).ToList()
            );
        }

        public void KeepBanForUser(User user)
        {
            if (user == null) return;
            UpdateUserRole(user, RoleType.Banned);
            IsAppealUserBanned = true;
            UserStatusDisplay = GetUserStatusDisplay(user, true);
            LoadStatistics();
        }

        public void AcceptAppealForUser(User user)
        {
            if (user == null) return;
            UpdateUserRole(user, RoleType.User);
            IsAppealUserBanned = false;
            UserStatusDisplay = GetUserStatusDisplay(user, false);
            LoadStatistics();
        }

        private void UpdateUserRole(User user, RoleType roleType)
        {
            _userService.UpdateUserRole(user.UserId, roleType);
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
            
            var reviews = GetUserReviews(selectedUser.UserId);
            UserReviewsWithFlags = new ObservableCollection<string>(
                reviews.Select(r => FormatReviewWithFlags(r)).ToList()
            );
        }

        public string GetUserStatusDisplay(User user, bool isBanned)
        {
            if (user == null) return string.Empty;
            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\nStatus: {(isBanned ? "Banned" : "Active")}";
        }

        public string FormatUserUpgradeInfo(User user, string currentRoleName, string requiredRoleName) 
        {
            if (user == null) return string.Empty;
            return $"User ID: {user.UserId}\nEmail: {user.EmailAddress}\n{currentRoleName} -> {requiredRoleName}";
        }

        public string FormatReviewContent(Review review)
        {
            if (review == null) return string.Empty;
            return $"{review.Content}";
        }

        public string FormatReviewWithFlags(Review review)
        {
            if (review == null) return string.Empty;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 