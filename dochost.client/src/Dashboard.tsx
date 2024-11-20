import {useQuery} from "@tanstack/react-query";

function Dashboard() {
    const { isPending, error, data, isFetching} = useQuery({
        queryKey: ['weatherforecast'],
        queryFn: async () => {
            const response = await fetch(
                "weatherforecast"
            )
            return await response.json();
        }
    });

    if (isPending) return 'Loading...'

    if (error) return 'An error has occurred: ' + error.message

    return (
        <>
            <h1>Doch Ost</h1>
            {data.map((forecast: any) => {
                return (
                    <div>{forecast.summary}</div>
                )
            })}
        </>
    )
}

export default Dashboard;