using PhonoArk.Mobile.Core.ViewModels;
using Xunit;

namespace PhonoArk.Mobile.Tests.ViewModels;

public class HomeViewModelTests
{
    [Fact]
    public void GoToLearning_RaisesNavigateToLearning()
    {
        var vm = new HomeViewModel();
        bool fired = false;
        vm.NavigateToLearning += () => fired = true;

        vm.GoToLearning();

        Assert.True(fired);
    }

    [Fact]
    public void GoToExam_RaisesNavigateToExam()
    {
        var vm = new HomeViewModel();
        bool fired = false;
        vm.NavigateToExam += () => fired = true;

        vm.GoToExam();

        Assert.True(fired);
    }

    [Fact]
    public void GoToLearning_NoSubscriber_DoesNotThrow()
    {
        var vm = new HomeViewModel();
        var exception = Record.Exception(() => vm.GoToLearning());
        Assert.Null(exception);
    }

    [Fact]
    public void GoToExam_NoSubscriber_DoesNotThrow()
    {
        var vm = new HomeViewModel();
        var exception = Record.Exception(() => vm.GoToExam());
        Assert.Null(exception);
    }
}
