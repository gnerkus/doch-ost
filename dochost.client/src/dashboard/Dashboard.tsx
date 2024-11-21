import {useQuery} from "@tanstack/react-query";
import {fetchDocuments} from "../core/http/api.ts";
import {useAuth} from "../auth/AuthContext.tsx";

function Dashboard() {
    const { session } = useAuth();
    const {
        isPending, error, data
    } = useQuery({
        queryKey: ['documents'],
        queryFn: fetchDocuments,
        enabled: !!session?.token
    });

    if (isPending) return 'Loading...'

    if (error) return 'An error has occurred: ' + error.message

    return (
        <>
            <h1>Doch Ost</h1>
            <div>
                {data.map(documentInfo => {
                    return (
                        <p>{documentInfo.fileName}</p>
                    )
                })}
            </div>
        </>
    )
}

export default Dashboard;