import {useQuery, useQueryClient} from "@tanstack/react-query";
import {downloadFile, fetchDocuments, getShareableLink, uploadFiles} from "../core/http/api.ts";
import {useAuth} from "../auth/AuthContext.tsx";
import {useForm} from "react-hook-form";
import {useState} from "react";

function Dashboard() {
    const [shareLink, setShareLink] = useState({});
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

    const getLink = async (fileId) => {
        const link = await getShareableLink(fileId);
        setShareLink({
            ...shareLink,
            [fileId]: link
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
                        <div>
                            <p>{documentInfo.fileName}</p>
                            <input type="text" value={shareLink[documentInfo.id] ? `https://localhost:7119/documents/file?share=${encodeURIComponent(shareLink[documentInfo.id])}` : ""} />
                            <button onClick={async () => {
                                await downloadFile(documentInfo.id, documentInfo.fileExt, documentInfo.fileName)
                            }}>Download</button>
                            <button onClick={async () => {
                                await getLink(documentInfo.id)
                            }} >Share</button>
                        </div>

                    )
                })}
            </div>
        </>
    )
}

export default Dashboard;