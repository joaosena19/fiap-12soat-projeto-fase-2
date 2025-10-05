using Moq;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Tests.Application
{
    public static class BasePresenterMockExtensions
    {
        public static void DeveTerApresentadoSucessoComQualquerObjeto<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.IsAny<TSucesso>()), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com um objeto de sucesso.");
        }

        public static void DeveTerApresentadoSucesso<TPresenter, TSucesso>(this Mock<TPresenter> mock, TSucesso objeto)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.Is<TSucesso>(o => Equals(o, objeto))), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com o objeto fornecido.");
        }

        public static void DeveTerApresentadoErro<TPresenter, TSucesso>(this Mock<TPresenter> mock, string mensagem, ErrorType errorType)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarErro(mensagem, errorType), Times.Once,
                $"Era esperado que o método ApresentarErro fosse chamado exatamente uma vez com a mensagem '{mensagem}' e tipo '{errorType}'.");
        }

        public static void NaoDeveTerApresentadoSucesso<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.IsAny<TSucesso>()), Times.Never,
                "O método ApresentarSucesso não deveria ter sido chamado.");
        }

        public static void NaoDeveTerApresentadoErro<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never,
                "O método ApresentarErro não deveria ter sido chamado.");
        }
    }
}
