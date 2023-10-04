namespace Utilities.Grid
{
    public class NodeCell : GridCell<int>
    {
        public int GCost;
        public int HCost;
        public bool IsWalkable = true;

        public NodeCell Parent;
        public int FCost => value;

        public override string ToString()
        {
            return $"{x},{y}";
        }

        public void CalculateFCost()
        {
            value = GCost + HCost;
        }
    }
}
