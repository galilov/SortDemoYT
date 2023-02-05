using System.Drawing;
using System.Drawing.Drawing2D;
using System.ServiceModel;

namespace AlgoDemo
{
    [ServiceContract]
    public interface IAlgoDemoService
    {
        [OperationContract]
        int GetItemCount();

        [OperationContract]
        int GetItem(int index);

        [OperationContract]
        void SwapItems(int index1, int index2);

        [OperationContract]
        void SetItem(int index, byte val);

        [OperationContract]
        void Select(int index, int argb);

        [OperationContract]
        void Unselect(int index);

        [OperationContract]
        void UnselectAll();
    }
}
