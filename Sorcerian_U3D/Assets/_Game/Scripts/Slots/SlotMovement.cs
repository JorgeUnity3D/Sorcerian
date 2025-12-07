namespace Kapibara.ConnectSlots
{
    public class SlotMovement
    {
        private Slot _slot;
        private int _targetRow;
        private int _targetColumn;

        public Slot Slot
        {
            get => _slot;
        }
        
        public int TargetRow
        {
            get => _targetRow;
        }
        
        public int TargetColumn
        {
            get => _targetColumn;
        }

        public SlotMovement(Slot slot, int targetRow, int targetColumn)
        {
            _slot = slot;
            _targetRow = targetRow;
            _targetColumn = targetColumn;
        }

        public override string ToString()
        {
            return $"<b><color=#FFD700>Slot</b> [{_slot}] - <b>TargetPosition</b></color> <color=#55FFFF>({_targetRow},{_targetColumn})</color>";
        }
    }
}