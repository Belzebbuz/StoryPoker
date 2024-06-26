﻿namespace StoryPoker.Client.Web.Api.Configurations;

public class PaymentConfig
{
    public required bool Enabled { get; init; }
}
// БЕСПЛАТНАЯ ВЕРСИЯ
// 1) КОЛ-ВО ИГРОКОВ МАКС 8
// 2) КОЛ-ВО ЗАДАЧ МАКС 8
//
// ПЛАТНАЯ ВЕРСИЯ ЧТО МОЖНО МОНЕТИЗИРОВАТЬ
//1) ЕСТЬ ВОЗМОЖНОСТЬ ГРУППИРОВАТЬ ИГРОКОВ
//2) КОЛ-ВО ЗАДАЧ
//3) КОЛ-ВО ГРУПП
//4) КОЛ-ВО

//АЛГОРИТМ
// 1) ПРОВЕРЯЕМ PaymentConfig.Enabled
// 2) ЕСЛИ FALSE ТО ДОСТУПНЫ ВСЕ ПЛАТНЫЕ ФУНКЦИИ БЕЗ ПРОВЕРКИ ПОЛЬЗОВАТЕЛЯ т.е. при создании комнаты устанавливаем enum MaxLevel
// 3) ЕСЛИ TRUE то при создании комнаты по id игрока находим пользователя. Проверяем его уровень оплаты и устанавливаем его.
