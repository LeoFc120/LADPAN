using UnityEngine;

public interface ICarriable
{
    // Método que cualquier objeto (auto, jugador, CPU) debe implementar
    // cuando necesite moverse a otro carril
    void MoveToLane(int laneIndex);
}