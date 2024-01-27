export default function RecuringExpences() {

    const expences = [
        {name: "electricity", amount: 10, paid: true},
        {name: "gas", amount: 20},
        {name: "mortgage", amount: 100, paid: true},
        {name: "phones", amount: 40, paid: true},
        {name: "car loan", amount: 50},
        {name: "water", amount: 30}
    ]

    function handleChange(e) {
        console.log(`checkbox changed `)
    }

    return (<>
        {expences.map((e) => 
            <div key={e.name}>
                <input checked={e.paid} type="checkbox" onChange={handleChange} />
                {e.name} {e.amount}
            </div>)}
    </>)

}