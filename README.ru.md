# MinChess

![Logo](https://raw.githubusercontent.com/MIN-Corp/MIN.Chess/master/chesslogo.png)

> **Многопользовательские шахматы с полной реализацией правил, таймером и поддержкой наблюдателей**

[![Stars](https://img.shields.io/github/stars/CasCadeVR/MIN.Chess?style=flat-square&logo=github)](https://github.com/MIN-Corp/MIN.Chess/stargazers)
[![Forks](https://img.shields.io/github/forks/CasCadeVR/MIN.Chess?style=flat-square&logo=github)](https://github.com/MIN-Corp/MIN.Chess/network/members)
[![License](https://img.shields.io/github/license/CasCadeVR/MIN.Chess?style=flat-square)](https://github.com/MIN-Corp/MIN.Chess/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Latest-blue?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

[English](README.md) • [Русский](README.ru.md)

---

## Содержание

- [Возможности](#-возможности)
- [Архитектура](#-архитектура)
- [Технологический стек](#-технологический-стек)
- [Установка и запуск](#-установка-и-запуск)
- [Структура проекта](#-структура-проекта)
- [Игровой процесс](#-игровой-процесс)
- [Правила](#-правила)
- [Авторы](#-авторы)

---

## Возможности

| Возможность | Описание |
|-------------|----------|
| **Полная реализация правил** | Все ходы фигур, рокировка, взятие на проходе, превращение пешки |
| **Таймер** | Настраиваемое время партии (1-30 мин) с добавкой (0-20 сек) |
| **Премувы** | Постановка ходов заранее, исполняются при наступлении очереди |
| **Наблюдатели** | Неограниченное количество зрителей одной партии |
| **Смена роли** | Наблюдатель может занять место свободного игрока |
| **Голосование** | Совместное решение о ничьей или рестарте |
| **Сдача** | Возможность признать поражение |
| **Сетевая игра** | Игра через локальную сеть (Named Pipes / TCP) |
| **Desktop UI** | Интуитивный интерфейс на WinForms с drag-and-drop |
| **Отображение взятых фигур** | Визуальный счётчик съеденных фигур оппонента |
| **Меню паузы** | Возможность поставить игру на паузу |

> [!IMPORTANT]
> MinChess работает на платформе **MIN** — через Named Pipes или TCP. Игроки и наблюдатели могут подключаться к одной партии из локальной сети.

---

## Архитектура

```mermaid
flowchart TB
    %%{init: {'flowchart': {'nodeSpacing': 12}}}%%
    classDef uiLayer fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:#1e40af
    classDef netLayer fill:#e0e7ff,stroke:#4f46e5,stroke-width:2px,color:#3730a3
    classDef coreLayer fill:#ede9fe,stroke:#7c3aed,stroke-width:2px,color:#5b21b6
    classDef transport fill:#f5f3ff,stroke:#c7c2ea,stroke-width:1.5px,color:#312e81
    classDef highlight fill:#c7d2fe,stroke:#4f46e5,stroke-width:3px,color:#312e81
    
    subgraph UI["Desktop Clients (WinForms)"]
        direction TB
        A["MIN.Chess.Desktop<br>Локальная игра"]
        B["MIN.Chess.Client<br>Сетевой клиент"]
    end
    
    subgraph NET["Network Layer"]
        direction TB
        C["MIN.Chess.Server<br>Игровой сервер"]
        D["RoomService<br>Управление комнатой"]
        E["GameService<br>Игровая логика"]
        F["TimerService<br>Таймер"]
        G["VoteService<br>Голосование"]
    end
    
    subgraph CORE["Core Layer"]
        direction TB
        H["Board<br>Доска и фигуры"]
        I["GameState<br>Состояние игры"]
        J["Moves<br>Система ходов"]
    end

    subgraph TRANS["Transport Layer"]
        direction TB
        L["Named Pipes"]
        M["TCP"]
    end
    
    class A highlight;
    class B,C,D,E,F,G,H,I,J,L,M transport
    class UI uiLayer;
    class NET netLayer;
    class CORE coreLayer;
    class TRANS transport
    
    UI --> NET
    NET --> CORE
    NET --> TRANS
```

### Модули

#### Core (Ядро)
| Модуль | Назначение |
|--------|------------|
| `MIN.Chess.Logic` | Полная реализация шахматной логики |

#### Desktop (UI)
| Модуль | Назначение |
|--------|------------|
| `MIN.Chess.Desktop` | Локальный десктопный клиент |

#### Network (Сеть)
| Модуль | Назначение |
|--------|------------|
| `MIN.Chess.Server` | Сервер партии, обработка сообщений |
| `MIN.Chess.Client` | Сетевой клиент для подключения к партии |
| `MIN.Chess.Messages` | Сообщения и протокол обмена |
| `MIN.Ipc.Transport.NamedPipes` | Транспорт через Named Pipes |
| `MIN.Ipc.Transport.Tcp` | Транспорт через TCP |

---

## Технологический стек

| Категория | Технология | Описание |
|-----------|------------|----------|
| Язык | C# | Современный объектно-ориентированный язык |
| Платформа | .NET 8.0 | Кроссплатформенный фреймворк |
| UI | WinForms | Настольный интерфейс Windows |
| Транспорт | Named Pipes / TCP | Межпроцессное и сетевое взаимодействие |
| Стиль | .editorconfig | Единый код-стайл |

---

## Установка и запуск

### Требования

> [!TIP]
> Убедитесь, что у вас установлен .NET SDK 8.0 или выше.

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) или выше
- Windows 10/11
- Платформа MIN (входит в репозиторий)

### Быстрый старт

```bash
# 1. Клонирование репозитория
git clone https://github.com/MIN-Corp/MIN.Chess.git
cd MinChess

# 2. Восстановление зависимостей
dotnet restore

# 3. Сборка проекта
dotnet build

# 4. Запуск сервера
dotnet run --project Network/MIN.Chess.Server/MIN.Chess.Server.csproj

# 5. Запуск клиента (в отдельном терминале)
dotnet run --project Network/MIN.Chess.Client/MIN.Chess.Client.csproj
```

---

## Структура проекта

```mermaid
flowchart TB
    %%{init: {'flowchart': {'nodeSpacing': 20}}}%%
    classDef coreFolder fill:#ede9fe,stroke:#7c3aed,color:#5b21b6
    classDef deskFolder fill:#dbeafe,stroke:#3b82f6,color:#1e40af
    classDef netFolder fill:#e0e7ff,stroke:#4f46e5,color:#3730a3
    classDef file fill:#fff,stroke:#c7d2fe,color:#4338ca
    
    subgraph ROOT["MinChess"]
        subgraph LOGIC["MIN.Chess.Logic/"]
            L1["Models/Board, Piece, GameState"]
            L2["Models/Moves/"]
            L3["Models/Pieces/"]
            L4["Enums/"]
            L5["Services/"]
            L6["Extensions/"]
        end
        subgraph DESK["MIN.Chess.Desktop/"]
            D1["Forms/"]
            D2["Components/"]
            D3["Services/"]
            D4["Assets/"]
        end
        subgraph NET["Network/"]
            N1["MIN.Chess.Server/"]
            N2["MIN.Chess.Client/"]
            N3["MIN.Chess.Messages/"]
            N4["Ipc/Transport/"]
        end
    end
    
    class LOGIC coreFolder
    class DESK deskFolder
    class NET netFolder
    class L1,L2,L3,L4,L5,L6,D1,D2,D3,D4,N1,N2,N3,N4 file
```

> [!NOTE]
> Настройки проекта централизованы через `Directory.Build.props` и `Directory.Packages.props`.

---

## Игровой процесс

1. **Старт партии** — два игрока занимают места белых и чёрных
2. **Настройка таймера** — белый игрок выбирает время партии и добавку
3. **Игра** — игроки ходят по очереди, наблюдатели следят за партией
4. **Премувы** — можно ставить ходы заранее; они исполнятся, когда дойдёт очередь
5. **Окончание** — мат, пат, сдача, тайм-аут, ничья, правило 50 ходов, троекратное повторение

### Управление

| Действие | Описание |
|----------|----------|
| **Drag-and-drop** | Перетаскивание фигуры мышью |
| **Сдаться** | Кнопка «Сдаться» |
| **Ничья** | Кнопка «Предложить ничью» |
| **Пауза** | Escape или кнопка «Пауза» |
| **Рестарт** | Голосование за перезапуск |
| **Обновить состояние** | На случай рассинхронизации |

---

## Правила

> [!IMPORTANT]
> MinChess поддерживает все официальные правила шахмат.

### Реализованные механики

| Правило | Статус |
|---------|--------|
| **Ходы фигур** | Все стандартные ходы короля, ферзя, ладьи, слона, коня и пешки |
| **Рокировка** | Короткая и длинная, с проверкой препятствий и прохода через битое поле |
| **Взятие на проходе** | En passant с валидацией легальности |
| **Превращение пешки** | Выбор фигуры при достижении последней горизонтали |
| **Шах и мат** | Обнаружение шаха, проверка легальности ходов |
| **Пат** | Автоматическое обнаружение |
| **Правило 50 ходов** | Ничья при отсутствии взятий и ходов пешками за 50 ходов |
| **Троекратное повторение** | Ничья при повторении позиции трижды |
| **Недостаток материала** | Ничья при невозможности поставить мат |

---

## Авторы

| Автор | Роль | Вклад |
|-------|------|-------|
| [**CasCadeVR**](https://github.com/CasCadeVR) | Основатель | Основной разработчик, создатель архитектуры |

---

> [!TIP]
> **Сделано с любовью командой CasCade**

*MinChess — шахматы для вашей сети*

---

[![Stars](https://img.shields.io/github/stars/MIN-Corp/MIN.Chess?style=social)](https://github.com/MIN-Corp/MIN.Chess)
[![Forks](https://img.shields.io/github/forks/MIN-Corp/MIN.Chess?style=social)](https://github.com/MIN-Corp/MIN.Chess)

[Исходный код](https://github.com/MIN-Corp/MIN.Chess) • 
[Сообщить об ошибке](https://github.com/MIN-Corp/MIN.Chess/issues) • 
[Лицензия MIT](LICENSE.txt)
