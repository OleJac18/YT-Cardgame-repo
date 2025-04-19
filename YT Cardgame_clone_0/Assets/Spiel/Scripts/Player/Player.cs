using System.Collections.Generic;
using Unity.Netcode;

[System.Serializable]
public class Player : INetworkSerializable
{
    public ulong id;
    public List<int> cards;
    public string name;
    public int score;

    public Player() { }

    public Player(ulong id, List<int> cards, string name, int score)
    {
        this.id = id;
        this.cards = cards;
        this.name = name;
        this.score = score;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref score);


        // Senden: Länge der Liste abspeichern, um diese danach in den Stream zu schreiben
        // Empfangen:  Es wird ein Defaultwert (0) in count geschrieben, weil 
        //         list.Count noch null sein könnte, da sie entweder keine Elemente
        //         hat oder noch nicht initalisiert worden ist
        int count = serializer.IsWriter ? cards.Count : 0;
        serializer.SerializeValue(ref count);

        // Für das Lesen muss die Liste initalisiert sein, bevor auf sie in der
        // for-Schleife zugegriffen wird. Könnte sonst null sein
        if (serializer.IsReader)
        {
            cards = new List<int>(count);
        }

        for (int i = 0; i < count; i++)
        {
            // Senden: Wert des Elements abspeichern, sofern der Index nicht 
            //         außerhalb des Bereichs der Liste ist
            // Empfangen:  Es wird ein Defaultwert (0) in card geschrieben, weil 
            //         noch keine Elemente in der Liste abgespeichert sind.
            int card = serializer.IsWriter && i < cards.Count ? cards[i] : 0;
            serializer.SerializeValue(ref card);

            // Beim Empfangen werden die einzelnen Elemente manuell in der Liste abgespeichert
            if (serializer.IsReader)
            {
                if (i < cards.Count)
                    cards[i] = card;
                else
                    cards.Add(card);
            }
        }
    }
}
