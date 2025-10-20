using System.Collections.Generic;
using Random = UnityEngine.Random;

public class W14_NumberGenerator
{
    // FIELD
    public W14_GameManager _gameManager;
    public int _questionCount;

    // FUNC
    /// <summary>
    /// Creates a random number.
    /// </summary>
    /// <param name="min">int min</param>
    /// <param name="max">int max</param>
    /// <returns>int number</returns>
    public int GetRandomResult(char operation, int level, int max)
    {
        int result = 0;

        switch (operation)
        {
            case '+':
                result = Random.Range(level * 2 + 4, (level + 1) * 5);
                break;
            case '-':
                result = Random.Range(level, level + 4);
                break;
            case 'x':
                do
                {
                    result = Random.Range((level + 4) * 2, (level + 7) * 4);
                } while (!IsResultFitForMultiplication(result) || result % 2 != 0);

                break;
            case ':':
                result = Random.Range(2, max / (level + 2) + 1);
                break;
        }

        return result;
    }

    /// <summary>
    /// Creates a list contains random numbers.
    /// </summary>
    /// <param name="min">int min</param>
    /// <param name="max">int max</param>
    /// <returns>List<int> numbers</returns>
    public List<int> GetFirstNumbers(int min, int max, int level, int result, char operation)
    {
        List<int> numbers = new List<int>();
        List<int> consecutiveNumbersForSubtraction = new List<int>((level + 3) * 2);
        List<int> consecutiveNumbersForDivision = new List<int>();

        int turn = 5;
        _questionCount = _gameManager.questionCount;

        // Çıkarma ve bölme işlemi için yeni bir ardışık sayı dizisi oluşturup; seviyeye bağlı,daha stabil sayılar kullanıldı.
        switch (operation)
        {
            case '-':
                {
                    for (int i = 0; i < consecutiveNumbersForSubtraction.Capacity; i++)
                    {
                        consecutiveNumbersForSubtraction.Add(result + i + 1);
                    }

                    break;
                }
            case ':':
                {
                    consecutiveNumbersForDivision.Capacity = max / result;
                    for (int i = 1; i <= consecutiveNumbersForDivision.Capacity; i++)
                    {
                        consecutiveNumbersForDivision.Add(result * i);
                    }

                    break;
                }
        }

        for (int i = 0; i < _questionCount; i++)
        {
            int number = Random.Range(min, max);
            int tempSum = result - number;
            /* Sayının son basamağı 7 ve ya 9 ise ilk seviyelerde yer almayacak. İlk sayilardan gelmese bile 2.sayı olarak
             gelebildiği için tempSum eklendi.*/
            switch (operation)
            {
                case '+' when number >= result || (level < 10 &&
                                                   (number % 10 == 7 || number % 10 == 9 || tempSum % 10 == 7 ||
                                                    tempSum % 10 == 9)):
                    i--;
                    continue;
                case '-':
                    {
                        number = consecutiveNumbersForSubtraction[Random.Range(0, consecutiveNumbersForSubtraction.Count)];
                        for (int j = 0; j < consecutiveNumbersForSubtraction.Count; j++)
                        {
                            if (consecutiveNumbersForSubtraction[j] == (number - 2 * result) ||
                                consecutiveNumbersForSubtraction[j] == (number - result) ||
                                consecutiveNumbersForSubtraction[j] == (number + result) ||
                                consecutiveNumbersForSubtraction[j] == (number + 2 * result))

                            {
                                consecutiveNumbersForSubtraction.Remove(consecutiveNumbersForSubtraction[j]);
                                j--;
                            }
                        }

                        break;
                    }
                case ':':
                    {
                        number = consecutiveNumbersForDivision[Random.Range(0, consecutiveNumbersForDivision.Count)];
                        for (int j = 0; j < consecutiveNumbersForDivision.Count; j++)
                        {
                            if (consecutiveNumbersForDivision[j] == (number * result * result) ||
                                consecutiveNumbersForDivision[j] == (number / result / result))
                            {
                                consecutiveNumbersForDivision.Remove(consecutiveNumbersForDivision[j]);
                            }
                        }

                        break;
                    }
                case 'x' when result % number != 0:
                    i--;
                    continue;
            }

            // Aynı sayılar gelmemesi için.
            if (turn != 0 && operation != '-')
            {
                int temp = 0;
                if (operation == '+')
                {
                    temp = result - number;
                }

                if (numbers.Contains(number) || numbers.Contains(temp))
                {
                    i--;
                    turn--;
                    continue;
                }
            }

            numbers.Add(number);
            turn = 5;
        }

        return numbers;
    }

    /// <summary>
    /// Generates numbers based on previously generated random numbers.
    /// </summary>
    /// <param name="firstNumbers">Lists<int> randomNumbers</param>
    /// <param name="result">int result</param>
    /// <returns>List<int> remainingNumbers</returns>
    public List<int> GetSecondNumbers(List<int> firstNumbers, int result, char operation)
    {
        List<int> secondNumbers = new List<int>();
        int secondNumber = 0;

        for (int i = 0; i < firstNumbers.Count; i++)
        {
            switch (operation)
            {
                case '+':
                    secondNumber = result - firstNumbers[i];
                    break;
                case '-':
                    secondNumber = firstNumbers[i] - result;
                    break;
                case 'x':
                    secondNumber = result / firstNumbers[i];
                    break;
                case ':':
                    secondNumber = firstNumbers[i] / result;
                    break;
            }

            secondNumbers.Add(secondNumber);
        }

        return secondNumbers;
    }

    /// <summary>
    /// For controlling the multipliers while generating a random result for the multiplication operation.
    /// </summary>
    /// <param name="result">int result</param>
    /// <returns></returns>
    private bool IsResultFitForMultiplication(int result)
    {
        int dividerCount = 0;

        for (int i = 2; i < 10; i++)
        {
            if (result % i == 0)
            {
                dividerCount++;
            }
        }

        return dividerCount > 2;
    }
}