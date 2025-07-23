using UnityEngine;

public class BluePrint : MonoBehaviour
{
    public string ItemName;

    public string Req1;
    public string Req2;

    public int Req1amount;
    public int Req2amount;

    public int numOfReq;

    public BluePrint(string name, int ReqNum, string R1, int R1num, string R2, int R2num)
    {
        ItemName = name;
        numOfReq = ReqNum;
        Req1 = R1;
        Req2 = R2;

        Req1amount = R1num;
        Req2amount = R2num;
    }
}
