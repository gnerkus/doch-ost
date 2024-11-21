import {useQuery, useQueryClient} from "@tanstack/react-query";
import {fetchDocuments, uploadFiles} from "../core/http/api.ts";
import {useAuth} from "../auth/AuthContext.tsx";
import {useForm} from "react-hook-form";

function Dashboard() {
    const { session } = useAuth();
    const {
        isPending, error, data
    } = useQuery({
        queryKey: ['documents'],
        queryFn: fetchDocuments,
        enabled: !!session?.token
    });
    const queryClient = useQueryClient();
    const { register, handleSubmit, formState: {errors} } = useForm();

    const onSubmit = (formData) => {
        uploadFiles(formData, () => {
            queryClient.invalidateQueries({ queryKey: ['documents']})
        });
    }

    if (isPending) return 'Loading...'

    if (error) return 'An error has occurred: ' + error.message

    return (
        <>
            <h1>Doch Ost</h1>
            <div>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <label htmlFor="files">Upload:</label>
                        <input
                            type="file"
                            id="files"
                            {...register("files", {required: true})}
                            multiple
                        />
                    </div>
                    <button type="submit">Upload</button>
                </form>
                {errors?.files && <span>File upload error</span> }
            </div>
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