export default function RecuringExpences() {

    const expences = [
        {name: "electricity", amount: 10},
        {name: "gas", amount: 20},
        {name: "mortgage", amount: 100},
        {name: "phones", amount: 40},
        {name: "car loan", amount: 50},
        {name: "water", amount: 30}
    ]

    return (<>
        {expences.map((e) => <div key={e.name}>{e.name} {e.amount}</div>)}
    </>)

}