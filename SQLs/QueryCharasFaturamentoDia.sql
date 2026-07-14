create table #temp
(
	data date,
	valor decimal(20,2) 
);

insert into #temp
SELECT        convert(date, TBPedidos.DataInicio), sum(TBAtribRefeicoesPedido.Valor + TBAtribRefeicoesPedido.Acrescimo)
FROM            TBAtribRefeicoesPedido INNER JOIN
                         TBPedidos ON TBAtribRefeicoesPedido.CodPedido = TBPedidos.Codigo
						 where TBPedidos.DataTermino is not null
group by  TBPedidos.DataInicio with ROLLUP

select data, sum(valor) from #temp
group by data
order by data asc

select data, valor from #temp
order by data asc

drop table #temp