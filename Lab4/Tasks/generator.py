# -*- coding: utf-8 -*-

from random import gauss, randrange

N = 4           # Número de processadores
TMT = 100       # Tempo médio da tarefa
Amount = 15     # Quantidade de tarefas
Sigma = 20      # Desvio padrão de TMT
Min = 50        # Mínimo valor de TMT
Max = 150       # Máximo valor de TMT
Type = 'Heavy'  # Tipo de carga (Heavy|Light)

print '# %s load, N = %d, TMT = %d' % (Type, N, TMT)

L = Amount * TMT / N
if Type == 'Light':
    Amount /= 3

instants = set()
while len(instants) != Amount:
    instants.add(randrange(L))
instants = sorted(instants)

i = 0
while i < Amount:
    processor = randrange(N)
    instant = instants[i]
    duration = int(round(gauss(TMT, Sigma)))
    if Min < duration and duration < Max:
        print '%d %d %d' % (processor, instant, duration)
        i += 1
