namespace RepairApp.Models;

public enum RepairStatus
{
    New,          // Новая заявка
    InProgress,   // В ремонте
    WaitingParts, // Ожидание запчастей
    Done,         // Готово
    Cancelled     // Отменено
}