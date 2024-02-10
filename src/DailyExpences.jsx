import Expence from "./Expence"

export default function ICanNameThisAnything() {

    const expences = [
        {name: "groceries", amount: 10, datetime: 1234567890},
        {name: "fuel", amount: 20, datetime: 1234567891},
        {name: "soda", amount: 1, datetime: 1234567892},
        {name: "lunch", amount: 5, datetime: 1234567893},
        {name: "clothes", amount: 40, datetime: 1234567894},
        {name: "movies", amount: 50, datetime: 1234567895},
        {name: "groceries", amount: 30, datetime: 1234567896}
    ]

    return (<>
        {
            expences.map((e) => 
                <>
                    <Expence amount={e.amount} key={e.datetime} category={e.name} description={e.name}   />
                </>
            )
        }
    </>)

}