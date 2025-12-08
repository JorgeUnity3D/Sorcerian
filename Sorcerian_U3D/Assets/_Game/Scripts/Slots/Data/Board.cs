namespace Kapibara.ConnectSlots
{
    public class Board
    {
        private readonly Slot[,] _slots;
        
        public Slot this[int row, int col]
        {
            get => (row < 0 || row >= Rows || col < 0 || col >= Columns) ? null : _slots[row, col];
            set => _slots[row, col] = value;
        }
        public int Rows
        {
            get => _slots.GetLength(0);
        }
        public int Columns
        {
            get => _slots.GetLength(1);
        }

        public Board(Slot[,] slots)
        {
            _slots = slots;
        }
    }
}