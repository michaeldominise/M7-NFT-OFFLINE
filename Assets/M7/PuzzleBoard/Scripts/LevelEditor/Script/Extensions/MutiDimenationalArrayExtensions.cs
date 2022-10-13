using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Serializable2DArray<T>
{
    [SerializeField] T[] array;
    [SerializeField] int columns;
    public int Columns 
    { 
        get { return columns; }
        set { ResizeArray(value, rows); columns = value; }
    }
    [SerializeField] int rows;
    public int Rows
    {
        get { return rows; }
        set { ResizeArray(columns, value); rows = value; }
    }
    public int Length { get { return columns * rows; } }

    public Serializable2DArray(int columns, int rows)
    {
        this.columns = columns;
        this.rows = rows;
        array = new T[columns * rows];
    }

    public T this[int column, int row]
    {
        get { return array[column * Rows + row]; }
        set { array[column * Rows + row] = value; }
    }

    public void ResizeArray(int newCols, int newRows)
    {
        var newArray = new T[newCols * newRows];
        int minCols = Mathf.Min(newCols, columns);
        int minRows = Mathf.Min(newRows, rows);

        for (int i = 0; i < minCols; i++)
            for (int j = 0; j < minRows; j++)
                newArray[i * j] = array[i * j];
        array = newArray;
        columns = newCols;
        rows = newRows;
    }
}

public static class MultiDimensionArrayExtensions {
    public static T[,] ResizeArray<T>(this T[,] original, int cols, int rows)
    {
        var newArray = new T[cols, rows];
        int minCols = Mathf.Min(cols, original.GetLength(0));
        int minRows = Mathf.Min(rows, original.GetLength(1));

        for (int i = 0; i < minCols; i++)
            for(int j = 0; j < minRows; j++)
               newArray[i, j] = original[i, j];
        return newArray;
    }
}
