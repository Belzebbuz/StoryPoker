using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using StoryPoker.Client.Web.Api.Extensions;
StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .ReadFrom.Configuration(builder.Configuration);
    });
    builder.Host.AddOrleansClient(builder.Configuration);
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configure =>
        {
            configure.LoginPath = "/api/account/login";
        });
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddCurrentUser();
    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCurrentUser();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}

// Каждый может зайти создать комнату
// Комната имеет название, тот кто создает может зайти в режиме ведущего
// В комнату может войти неограниченное кол-во участников/групп/заданий (до платной версии)
// Когда входит новый игрок, появляется запрос на имя
// В центре расположена ссылка на задачу
// Справа список задач
// Слева список игроков
// Текущий игрок обозначен (стрелка/цвет)
// Снизу показаны возможные кликабельные оценки (карточки с цифрами фибоначи)
// У ведущего(кто создал) есть кнопка Добавить(создает новую игру)
// У ведущего(кто создал) есть кнопка Начать(начинает оценку)
// После начала игроки выбирют карты, до окончания игры скрыты выбранные карты, выбор можно менять
// У ведущего есть кнопка открыть карты(показывает карты игроков)
// После вскрытия выводится округленное в большую стороны из чисел фибоначи число
//
/* Сценарий №1
 * 1) Ведущий открывает главную страницу
 * 2) Жмет кнопку создать сессию
 * 3) Вводит название сессии нпример BE
 * 4) Он переходит на страницу сессии и получает ссылку на комнату. Автоматически выбирается режим ведущего
 * 5) Ведущий делится ссылкой
 * 6) Игрок переходит поссылке, попадает на окно комнаты, но оно заблокировано пока он не введет имя
 * 7) После входа он отображается в списке игроков справа.
 * 8) Как только все игроки собрались Ведущий жмет кнопку добавить задачу, указывает ссылку и описание если надо
 * 9) После добавления, карточка задачи отображается в центре как текущая для оченки и в списке справа.
 * 10) Он может продолжить добавлять задачи, оцениваться будет задача, которая находится в центре
 * 11) Ведущий жмет "начало" и игроки начинают выбирать карточки
 * 12) Игроки выбирают карту и по желанию жмут кнопку подтвердить(чтобы показать готовность и лишний раз не спрашивать кто готов)
 * 13) Ведущий жмет "Раскрыть карты" и карточки игроков переворачиваются и автоматически показывается оценка
 * 14) Расчитанная оценка появлятся на карточке задачи, игроки в ходе обсуждения могут менять оценку и автоматически будет просиходить перерасчет
 *
 * Фичи:
 * 1) Одна комната(одна текущая задача) несколько групп
 * 2) аккаунт - хранить созданные сессии
 *
 */
