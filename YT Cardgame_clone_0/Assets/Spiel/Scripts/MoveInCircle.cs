using UnityEngine;

public class MoveInCircle
{
    //public GameObject card;
    //public Transform endPosition;
    /*private void Start()
    {
        //Invoke("LTMoveInCircle", 1f);
    }*/

    // Ein Objekt mit mehreren Zwischenpunkten zu einem Zielpunkt bewegen
    // Z.B. um es in einem Kreis zu bewegen
    // public void LTMoveInCircle()
    // {
    //     Vector3[] points = CalculateCircle(8, card.transform, endPosition.transform, -1, 100);

    //     LeanTween.moveSpline(card, points, 2.0f);
    // }

    ///  <summary>
    /// Berechnet die einzelnen Vektoren zwischen dem Start und Endpunkt in einem Kreisbogen.
    /// rotation : ist entweder 1 (gegen den Uhrzeigersinn) und -1 (im Uhrzeigersinn)
    /// maxHeight: gibt an wie groß der zweite Radius der Ellipse ist. 0 = kein Bogen, 100 = kleiner Bogen, 300 = zu erkennender Bogen
    /// </summary>
    /// <returns> Berechnete Punkte/Vectoren </returns>
    public static Vector3[] CalculateCircle(int amountOfPoints, Transform startPosition, Transform endPosition, int rotation, int maxHeight)
    {
        Vector3[] points = new Vector3[amountOfPoints + 3];       // Vektor der zurückgegeben wird, mit allen Punkten, zu denen sich das Objekt bewegen soll
        points[0] = startPosition.position;                     // Die startPosition wird zweimal abgespeichert, weil die erste in der Funktion LeanTween.moveSpline nur für die Rotation zuständig ist und nicht für die Position
        points[amountOfPoints + 2] = endPosition.position;        // Gleiche wie bei der Startposition in der Zeile drüber
        float radius = (Vector3.Distance(startPosition.position, endPosition.position)) / 2;      // Radius vom Mittelpunkt der beiden Punkte
        Vector3 middlePoint = startPosition.position - ((startPosition.position - endPosition.position) / 2);   // Position der mittleren Position
        Vector2 middlePoint2D = new Vector2(middlePoint.x, middlePoint.y);      // Mittlere Position ohne z-Position
        Vector2 startPosition2D = new Vector2(startPosition.position.x, startPosition.position.y);  // StartPosition ohne z-Position
        float correctionAngle = Mathf.Atan2(startPosition2D.y - middlePoint2D.y, startPosition2D.x - middlePoint2D.x);   // Startwinkel, der zwischen der Startposition und dem Mittelpunkt besteht
        //Debug.Log("CorrectionAngle: " + Mathf.Rad2Deg * correctionAngle);

        // Für die angegebenen Punkte werden gleichmäßig Punkte auf einem Kreisbogen erstellt
        for (int i = 0; i <= amountOfPoints; i++)
        {
            float angle = Mathf.Deg2Rad * (180f / amountOfPoints * i);

            // Bestimmung der horizontalen Wertes des Punktes auf der Ellipse
            float horizontalWithoutCorrect = Mathf.Cos(angle);

            // Bestimmung des vertikalen Wertes des Punktes auf der Ellipse
            // Aus der Formal für eine Ellipse abgeleitet:
            // a = radius1, b = radius2
            // (x²/a²) + (y²/b²) = 1 => y = b * sqrt(1 - (x²/a²))
            // a = radius, b = maxHeight
            // x = horizontalWithoutCorrect * radius = horizontalWithoutCorrect * a
            // y = b * sqrt(1 - (x²/a²)) => y = b * sqrt(1 - ((horizontalWithoutCorrect * a)²/a²)) => y = b * sqrt(1 - horizontalWithoutCorrect²)
            float verticalWithoutCorrect = rotation * (maxHeight * Mathf.Sqrt(1 - Mathf.Pow(horizontalWithoutCorrect, 2)));

            // Der horizontale Wert ist nur ein Wert zwischen 0 und 1
            // Damit der richtige Wert berechnet wird, muss dieser noch mal den Radius gerechnet werden
            horizontalWithoutCorrect = horizontalWithoutCorrect * radius;

            // Rotation der Ellipse
            // Hergeleitet mit der Formal für eine Rotation einer Ellipse mithilfe einer Drehmatrix
            // (x')   (cos(alpha) -sin(alpha))   (x)
            // ( )  = (                      ) * ( )
            // (y')   (sin(alpha)  cos(alpha))   (y)
            // x' = cos(alpha) * x + (-sin(alpha)) * y
            // y' = sin(alpha) * x + cos(alpha) * y
            float horizontal = Mathf.Cos(correctionAngle) * horizontalWithoutCorrect + (-Mathf.Sin(correctionAngle) * verticalWithoutCorrect);
            float vertical = Mathf.Sin(correctionAngle) * horizontalWithoutCorrect + Mathf.Cos(correctionAngle) * verticalWithoutCorrect;

            // Speichert die berechneten horizontalen (x) und vertikalen (y) Werte in einem Vector ab
            Vector3 direction = new Vector3(horizontal, vertical, 0);

            Vector3 subpoint = middlePoint + direction;

            //Creates an object on the place for debugging
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.parent = this.transform;
            // sphere.transform.position = subpoint;
            // sphere.transform.localScale = new Vector3(40f, 40f, 40f);

            points[i + 1] = subpoint;
        }
        return points;
    }
}